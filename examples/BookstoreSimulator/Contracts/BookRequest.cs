using FluentValidation;

namespace BookstoreSimulator.Contracts
{
    public class BookRequest
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public DateTime PublicationDate { get; set; }
        public int Quantaty { get; set; } = 0;
    }

    public class BookRequestValidator: AbstractValidator<BookRequest>
    {
        public BookRequestValidator()
        {
            RuleFor(book =>  book.Title).NotEmpty().WithMessage("Book title cannot be empty");
            RuleFor(book =>  book.Author).NotEmpty().WithMessage("Book author cannot be empty");

            RuleFor(book =>  book.PublicationDate)
                .NotEmpty().WithMessage("Book publication date cannot be empty")
                .LessThan(DateTime.UtcNow).WithMessage("Book publication date cannot be more than today");
        }
    }
}
