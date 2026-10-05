using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using vulnerable_api.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddAntiforgery(options =>
{
    options.SuppressXFrameOptionsHeader = true;
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy
            .WithOrigins("http://localhost:5500")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });     
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = "vulnerable-api",
            ValidAudience = "vulnerable-api",

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes("super-secret-development-key-12345"))
        };
    });

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=vulnerable-api.db"));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Lab safety guard: the remote-code-execution endpoints must never be reachable
// from another website open in your browser, or from another machine.
// The intentional flaws stay exploitable from curl, Burp Suite and the NovaBank frontend.
var dangerousPaths = new[] { "/api/execution", "/api/users/ping", "/api/users/upload" };
var trustedOrigins = new[] { "http://localhost:5500", "http://127.0.0.1:5500", "http://localhost:5066", "https://localhost:7098" };

app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value ?? "";
    if (dangerousPaths.Any(p => path.StartsWith(p, StringComparison.OrdinalIgnoreCase)))
    {
        var remote = context.Connection.RemoteIpAddress;
        var fetchSite = context.Request.Headers["Sec-Fetch-Site"].ToString();
        var origin = context.Request.Headers.Origin.ToString();

        var notLoopback = remote is null || !System.Net.IPAddress.IsLoopback(remote);
        var crossSite = fetchSite == "cross-site";
        var foreignOrigin = origin != "" && !trustedOrigins.Contains(origin);

        if (notLoopback || crossSite || foreignOrigin)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync("Blocked by lab safety guard: local requests only.");
            return;
        }
    }
    await next();
});

app.UseCors("FrontendPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild",
    "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();

    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, int TemperatureF, string? Summary)
{
    public WeatherForecast(DateOnly date, int temperatureC, string? summary)
        : this(date, temperatureC, 32 + (int)(temperatureC / 0.5556), summary)
    {
    }
}

