using BookstoreSimulator.Contracts;
using BookstoreSimulator.Infra.DAL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookstoreSimulator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly BookRequestValidator _bookRequestValidator;
        private readonly BookRepository _bookRepository;

        public BooksController(BookRepository bookRepository, BookRequestValidator bookRequestValidator)
        {
            _bookRepository = bookRepository;
            _bookRequestValidator = bookRequestValidator;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IResult> CreateBook([FromBody] BookRequest request)
        {
            var validationResult = _bookRequestValidator.Validate(request);
            if (validationResult.IsValid)
            {
                var bookId = Guid.NewGuid();
                var book = BookDBRecord.Create(request, bookId);
                var insertedResult = await _bookRepository.InsertBook(book);
                if (insertedResult)
                    return Results.Ok();
                else
                    return Results.StatusCode(StatusCodes.Status500InternalServerError);
            }
            else
                return Results.ValidationProblem(validationResult.ToDictionary());
        }

        [Authorize]
        [HttpGet]
        public async Task<IResult> Get(bool availableOnly = true)
        {
            var books = await _bookRepository.Get(availableOnly);
            if (books.Count > 0)
            {
                var data = new Response<List<BookDBRecord>>(books);
                return Results.Ok(data);
            }
            else
                return Results.StatusCode(StatusCodes.Status204NoContent);
        }
    }
}
