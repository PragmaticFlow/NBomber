using Microsoft.AspNetCore.Mvc;
using BookstoreSimulator.Infra.DAL;
using Microsoft.AspNetCore.Authorization;

namespace BookstoreSimulator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DatabasesController : ControllerBase
    {
        private readonly DB _db;

        public DatabasesController(DB db)
        {
            _db = db;
        }

        [AllowAnonymous]
        [HttpPut]
        public async Task PreparedDB()
        {
            _db.CleanTables();
            _db.CreateTables();
        }
    }
}
