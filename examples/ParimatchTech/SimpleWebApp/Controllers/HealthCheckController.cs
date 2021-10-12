using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace SimpleWebApp.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [ApiController]
    public class HealthCheckController
    {
        [Route("api/[action]")]
        public async Task<string> HealthCheck()
        {
            return await Task.FromResult("OK");
        }
    }
}