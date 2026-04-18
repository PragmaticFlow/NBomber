using HttpApiSimulator.Contracts;
using HttpApiSimulator.Infra.DAL;
using Microsoft.AspNetCore.Mvc;

namespace HttpApiSimulator.Controllers;

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
    public async Task<IResult> Get(int id)
    {
        var user = await _repository.GetById(id);
        return user == null
            ? Results.NotFound()
            : Results.Ok(user);
    }

    [HttpPost]
    public async Task<bool> Post([FromBody] User user)
    {
        return await _repository.Update(user);
    }

    [HttpPut("{id}")]
    public async Task<bool> Put(int id, [FromBody] UpdateUserReq request)
    {
        return await _repository.Update(request.ToUser(id));
    }
}
