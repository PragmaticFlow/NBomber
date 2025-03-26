using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;

namespace WebAppSimulator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CookiesAuthenticationController : ControllerBase
    {
        private readonly ConcurrentDictionary<string, Guid> GeneratedCookies = new();

        public CookiesAuthenticationController() { }

        [HttpPost]
        public Task Login(string login, string password)
        {

        }

        [HttpGet]
        public Task<string> GetAccess()
        {

        }
    }
}
