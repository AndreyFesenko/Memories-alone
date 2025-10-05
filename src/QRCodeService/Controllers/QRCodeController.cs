using Microsoft.AspNetCore.Mvc;
using QRCoder;

namespace QRCodeService.Controllers;

[ApiController]
[Route("api")]
public class QRCodeController : ControllerBase
{
    [HttpGet("generate")]
    public IActionResult Generate([FromQuery] string url, [FromQuery] int pixels = 300)
    {
        if (string.IsNullOrWhiteSpace(url)) return BadRequest(new { error = "url is required" });
        using var gen = new QRCodeGenerator();
        using var data = gen.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
        using var png = new PngByteQRCode(data);
        var pngBytes = png.GetGraphic(pixels / 25);
        return File(pngBytes, "image/png");
    }

    [HttpGet("generate-dataurl")]
    public IActionResult GenerateDataUrl([FromQuery] string url, [FromQuery] int pixels = 300)
    {
        if (string.IsNullOrWhiteSpace(url)) return BadRequest(new { error = "url is required" });
        using var gen = new QRCodeGenerator();
        using var data = gen.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
        using var png = new PngByteQRCode(data);
        var pngBytes = png.GetGraphic(pixels / 25);
        var b64 = Convert.ToBase64String(pngBytes);
        return Ok(new { dataUrl = $"data:image/png;base64,{b64}" });
    }
}
