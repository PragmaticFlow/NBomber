using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;

namespace WebAppSimulator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CookiesAuthenticationController : ControllerBase
    {
        private readonly ConcurrentBag<Guid> GeneratedCookies = new();

        public CookiesAuthenticationController() { }

        [HttpPost]
        public Task Login(string login, string password)
        {
            var authCookie = Guid.NewGuid();

            GeneratedCookies.Add(authCookie);
            Response.Cookies.Append("auth_hash_cookie", authCookie.ToString());

            return Task.CompletedTask;
        }

        [HttpGet]
        public Task<string> GetAccess()
        {
            var authCookie = Request.Cookies["auth_hash_cookie"];
            var convertResult = Guid.TryParse(authCookie, out Guid guidAuthCookie);

            if (convertResult)
            {
                if (GeneratedCookies.TryPeek(out guidAuthCookie))
                    return Task.FromResult("Authentication successful.");
                else
                    return Task.FromResult("Authentication failed. The auth_hash_cookie is wrong.");
            }
            else
            {
                return Task.FromResult("The auth_hash_cookie is not valid.");
            }            
        }
    }
}
