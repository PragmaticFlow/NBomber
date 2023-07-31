using BookstoreSimulator.Contracts;
using BookstoreSimulator.Infra;
using BookstoreSimulator.Infra.Bookstore;
using BookstoreSimulator.Infra.DAL;
using BookstoreSimulator.Infra.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace BookstoreSimulator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserRepository _rep;
        private readonly JwtSetings _jwtSetings;
        private readonly SingUpUserRequestValidator _singUpUserRequestValidator;
        private readonly LoginUserRequestValidator _loginUserRequestValidator;
   
        public UsersController
            (UserRepository rep, JwtSetings jwtSetings,
            SingUpUserRequestValidator singUpUserRequestValidator,
            LoginUserRequestValidator loginUserRequestValidator
            )
        {
            _rep = rep;
            _jwtSetings = jwtSetings;
            _singUpUserRequestValidator = singUpUserRequestValidator;
            _loginUserRequestValidator = loginUserRequestValidator;
        }

        [Route("singup")]
        [AllowAnonymous]
        [HttpPost]
        public async Task<IResult> Singup([FromBody] SingUpUserRequest request)
        {
            var validationResult = _singUpUserRequestValidator.Validate(request);
            if (validationResult.IsValid)
            {
                var (passwordHash, passwordSalt) = Password.HashPassword(request.Password);
                var createdDT = DateTime.UtcNow;
                var userId = Guid.NewGuid();
                var user = UserDBRecord.Create(request, userId, passwordHash, passwordSalt, createdDT, createdDT);

                var insertedResult = await _rep.InsertUser(user);

                if (insertedResult == DBResultExeption.Ok)
                    return Results.StatusCode(StatusCodes.Status200OK);
                else if (insertedResult == DBResultExeption.Duplicate)
                    return Results.StatusCode(StatusCodes.Status409Conflict);
                else
                    return Results.StatusCode(StatusCodes.Status500InternalServerError);
            }
            else
                return Results.ValidationProblem(validationResult.ToDictionary());
        }

        [Route("login")]
        [AllowAnonymous]
        [HttpPost]
        public async Task<IResult> Login([FromBody] LoginUserRequest request)
        {
            var validationResult = _loginUserRequestValidator.Validate(request);
            if (validationResult.IsValid)
            {
                var result = await _rep.TryFindUserLoginData(request.Email);
                if (result != null)
                {
                    var passwordValid = Password.VerifyPassword(request.Password, result.PasswordHash, result.PasswordSalt);
                    if (passwordValid)
                    {
                        var jwt = JwtToken.GenerateJwtToken(result.UserId.ToString(), _jwtSetings);
                        var response = new Response<string>(jwt);
                        return Results.Ok(response);
                    }
                    else
                        return Results.StatusCode(StatusCodes.Status401Unauthorized);
                }
                else
                    return Results.StatusCode(StatusCodes.Status404NotFound);
            }
            else
                return Results.ValidationProblem(validationResult.ToDictionary());
        }

        [Route("logout")]
        [Authorize]
        [HttpPost]
        public async Task<IResult> Logout()
        {
            return Results.Ok();
        }
    }
}
