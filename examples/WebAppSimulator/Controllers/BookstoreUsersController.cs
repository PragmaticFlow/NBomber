using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebAppSimulator.Contracts;
using WebAppSimulator.Contracts.Bookstore;
using WebAppSimulator.Infra.Bookstore;
using WebAppSimulator.Infra.Bookstore.DAL;

namespace WebAppSimulator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookstoreUsersController : ControllerBase
    {
        private BookstoreUserRepository _rep;
        private JwtSetings _jwtSetings;

        public BookstoreUsersController(BookstoreUserRepository rep, JwtSetings jwtSetings)
        {
            _rep = rep;
            _jwtSetings = jwtSetings;
        }

        //[HttpGet]
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //[HttpGet("{id}")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        [Route("singup")]
        [AllowAnonymous]
        [HttpPost]
        public async Task<IResult> Singup([FromBody] SingUpUserRequest request)
        {
            var validator = new SingUpUserRequestValidator();
            var validationResult = validator.Validate(request);
            if (validationResult.IsValid)
            {
                var (passwordHash, passwordSalt) = Password.HashPassword(request.Password);
                var createdDT = DateTime.UtcNow;
                var userId = Guid.NewGuid();
                var user = UserDBRecord.Create(request, userId, passwordHash, passwordSalt, createdDT, createdDT);

                if(await _rep.InsertUser(user))
                    return Results.StatusCode(StatusCodes.Status200OK);
                else
                    return Results.StatusCode(StatusCodes.Status409Conflict);
            }
            else
                return Results.ValidationProblem(validationResult.ToDictionary());
        }

        [Route("login")]
        [AllowAnonymous]
        [HttpPost]
        public async Task<IResult> Login([FromBody] LoginUserRequest request)
        {
            var validator = new LoginUserRequestValidator();
            var validationResult = validator.Validate(request);
            if (validationResult.IsValid)
            {
                var result = await _rep.TryFindUserLoginData(request.Email);
                if(result != null)
                {
                    var passwordValid = Password.VerifyPassword(request.Password, result.PasswordHash, result.PasswordSalt);
                    if (passwordValid)
                    {
                        var jwt = GenerateJwtToken(result.UserId.ToString());
                        var response = new Response<string>(jwt);
                        return Results.Ok(response);
                    }
                    else
                        return Results.StatusCode(StatusCodes.Status401Unauthorized);
                }
                else
                    return Results.StatusCode(StatusCodes.Status401Unauthorized);
            }
            else
                return Results.ValidationProblem(validationResult.ToDictionary());
        }

        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}

        private string GenerateJwtToken(string userName)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSetings.Key);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", userName) }),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = _jwtSetings.Issuer,
                Audience = _jwtSetings.Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
