using System.Collections.Concurrent;
using Microsoft.AspNetCore.Mvc;

namespace HttpApiSimulator.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CookiesAuthenticationController : ControllerBase
{
    private static readonly ConcurrentBag<Guid> GeneratedCookies = new();

    public CookiesAuthenticationController() { }

    [HttpPost]
    public IActionResult Login([FromBody] LoginDto loginDto)
    {
        var authCookie = Guid.NewGuid();

        GeneratedCookies.Add(authCookie);
        Response.Cookies.Append("auth_hash_cookie", authCookie.ToString());

        return Ok("Cookies were added.");
    }

    [HttpGet]
    public IActionResult GetData()
    {
        var authCookie = Request.Cookies["auth_hash_cookie"];
        var convertResult = Guid.TryParse(authCookie, out Guid guidAuthCookie);

        if (convertResult)
        {
            if (GeneratedCookies.TryPeek(out guidAuthCookie))
                return Ok("Authentication successful.");
            else
                return Unauthorized("Authentication failed. The auth cookie is wrong.");
        }
        else
        {
            return Unauthorized("The auth cookie is not valid.");
        }
    }
}

public class LoginDto
{
    public string Login { get; set; }
    public string Password { get; set; }
}
