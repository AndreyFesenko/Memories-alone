using Microsoft.AspNetCore.Mvc;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    protected IActionResult Error(string message, int code = 400) =>
        StatusCode(code, new { error = message, traceId = HttpContext.TraceIdentifier });
}
