using Microsoft.AspNetCore.Mvc;

namespace vulnerable_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PathTraversalController : ControllerBase
{
[HttpGet("file")]
public IActionResult GetFile(string filename)
{
    if (filename.Contains("../"))
    {
        return BadRequest("Path traversal detected");
    }
         filename = Uri.UnescapeDataString(filename);
    var baseDirectory = Path.Combine(
        Directory.GetCurrentDirectory(),
        "path-lab"
    );

    var filePath = Path.Combine(
        baseDirectory,
        filename
    );

    if (!System.IO.File.Exists(filePath))
    {
        return NotFound();
    }

    var content = System.IO.File.ReadAllText(filePath);

    return Ok(content);
}
[HttpGet("file-nonrecursive")]
public IActionResult GetFileNonRecursive(string filename)
{
    filename = filename.Replace("../", "");

    var baseDirectory = Path.Combine(
        Directory.GetCurrentDirectory(),
        "path-lab"
    );

    var filePath = Path.Combine(
        baseDirectory,
        filename
    );

    if (!System.IO.File.Exists(filePath))
    {
        return NotFound();
    }

    var content = System.IO.File.ReadAllText(filePath);

    return Ok(content);
}
[HttpGet("file-startvalidation")]
public IActionResult GetFileStartValidation(string filename)
{
    var baseDirectory = Path.Combine(
        Directory.GetCurrentDirectory(),
        "path-lab"
    );

    if (!filename.StartsWith(baseDirectory))
    {
        return BadRequest("Invalid path");
    }

    var filePath = Path.Combine(
        baseDirectory,
        filename
    );

    if (!System.IO.File.Exists(filePath))
    {
        return NotFound();
    }

    var content = System.IO.File.ReadAllText(filePath);

    return Ok(content);
}
[HttpGet("file-extension")]
public IActionResult GetFileExtension(string filename)
{
    var baseDirectory = Path.Combine(
        Directory.GetCurrentDirectory(),
        "path-lab"
    );

    // Naive extension validation
    if (!filename.EndsWith(".txt"))
    {
        return BadRequest("Only .txt files are allowed");
    }

    // Simulate legacy null-byte truncation
    var nullIndex = filename.IndexOf('\0');

    if (nullIndex >= 0)
    {
        filename = filename.Substring(0, nullIndex);
    }

    var filePath = Path.Combine(
        baseDirectory,
        filename
    );

    if (!System.IO.File.Exists(filePath))
    {
        return NotFound();
    }

    var content = System.IO.File.ReadAllText(filePath);

    return Ok(content);
}
}