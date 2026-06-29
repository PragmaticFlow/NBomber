using HttpApiSimulator.Infra.DAL;
using Microsoft.AspNetCore.Mvc;

namespace HttpApiSimulator.Controllers;

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
        _repository.DeleteTable();
        _repository.CreateDB();
    }
}
