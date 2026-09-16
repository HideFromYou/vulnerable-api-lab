using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Scripting;

namespace vulnerable_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExecutionController : ControllerBase
{
    [HttpPost("run")]
    public async Task<IActionResult> Run(string path)
    {
        var code = await System.IO.File.ReadAllTextAsync(path);

        var result = await CSharpScript.EvaluateAsync<string>(code);

        return Ok(result);
    }
}