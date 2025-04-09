using Microsoft.AspNetCore.Mvc;
using WebAppSimulator.Contracts;
using WebAppSimulator.Infra.DAL;

namespace WebAppSimulator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _repository;

        public UsersController(IUserRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("{id}")]
        public Task<User> Get(int id)
        {
            return _repository.GetById(id);
        }

        [HttpPut("{id}")]
        public async Task<bool> Put(int id, [FromBody] UserDto request)
        {
            return await _repository.Update(request.ToUser(id));
        }
    }
}
