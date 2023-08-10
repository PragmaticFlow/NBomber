namespace Demo.HTTP.SimpleBookstore.Contracts
{
    public class Book
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public DateTime PublicationDate { get; set; }
        public int Quantaty { get; set; }
    }

    public class BookResponse
    {
        public Guid BookId { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public DateTime PublicationDate { get; set; }
        public int Quantaty { get; set; }
    }
}
