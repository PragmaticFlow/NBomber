using BookstoreSimulator.Contracts;
using BookstoreSimulator.Infra;
using BookstoreSimulator.Infra.DAL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.PortableExecutable;

namespace BookstoreSimulator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly OrderRepository _repository;

        public OrdersController(OrderRepository repository)
        {
            _repository = repository;
        }

        [Authorize]
        [HttpPost]
        public async Task<IResult> CreateOrder([FromBody] OrderRequest request)
        {
            var userId = ExtractUserId(this.HttpContext.Request.Headers);

            if (userId != null)
            {
                var order = new OrderDBRecord(request, userId.Value);

                var createdOrder = await _repository.CreateOrder(order);

                if (createdOrder)
                    return Results.StatusCode(StatusCodes.Status200OK);
                else
                    return Results.StatusCode(StatusCodes.Status409Conflict);
            }
            else
                return Results.StatusCode(StatusCodes.Status400BadRequest);
        }

        private Guid? ExtractUserId(IHeaderDictionary headers)
        {
            Microsoft.Extensions.Primitives.StringValues jwtToken;

            if (headers.TryGetValue("Authorization", out jwtToken))
            {
                var token = jwtToken.ToString().Remove(0, "Bearer ".Length);
                var userIdString = JwtToken.DecodeJwtToken(token);
                Guid userGuid;
                if (Guid.TryParse(userIdString, out userGuid))
                {
                    return userGuid;
                }
                else
                    return null;
            }
            else
                return null;
        }
    }
}
