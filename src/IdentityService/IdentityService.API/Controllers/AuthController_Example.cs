using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = "AccountHolder")]
[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    [HttpGet("only-for-owners")]
    public IActionResult OnlyForOwners() => Ok("Видят только владельцы аккаунта");
}
