using Microsoft.AspNetCore.Mvc;
using WebAppSimulator.Infra.DAL;

namespace WebAppSimulator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataBasesController : ControllerBase
    {
        private readonly IUserRepository _repository;

        public DataBasesController(IUserRepository repository)
        {
            _repository = repository;
        }

        [HttpPut]
        public void PrepareDB()
        {
            _repository.DeleTable();
            _repository.CreateDB();
        }
    }
}
