using System.Text.Json.Serialization;

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
        [JsonPropertyName("bookId")]
        public Guid BookId { get; set; }
        [JsonPropertyName("title")]
        public string Title { get; set; }
        [JsonPropertyName("author")]
        public string Author { get; set; }
        [JsonPropertyName("publicationDate")]
        public DateTime PublicationDate { get; set; }
        [JsonPropertyName("quantaty")]
        public int Quantaty { get; set; }
    }

    public class BookListResponse
    {
        [JsonPropertyName("data")]
        public List<BookResponse> Data { get; set; }
    }
}
