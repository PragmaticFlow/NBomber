using Npgsql;
using Dapper;
using BookstoreSimulator.Contracts;

namespace BookstoreSimulator.Infra.DAL
{
    public class BookDBRecord
    {
        public Guid BookId { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public DateTime PublicationDate { get; set; }
        public int Quantaty { get; set; } = 0;

        internal static BookDBRecord Create(BookRequest request, Guid bookId)
        {
            return new BookDBRecord
            {
                BookId = bookId,
                Title = request.Title,
                Author = request.Author,
                PublicationDate = request.PublicationDate,
                Quantaty = request.Quantaty,
            };
        }
    }
    public class BookRepository
    {
        private string _connectionStr;

        public BookRepository(BookstoreSettings settings)
        {
            _connectionStr = settings.ConnectionString;
        }

        public async Task<bool> InsertBook(BookDBRecord record)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionStr))
                {
                    connection.Open();

                    var commandText = @"INSERT INTO Books 
                                    (BookId, Title, Author, PublicationDate, Quantaty) 
                                    VALUES (@BookId, @Title, @Author, @PublicationDate, @Quantaty)";
                    await connection.ExecuteAsync(commandText, record);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<BookDBRecord>> Get(bool availableOnly)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionStr))
                {
                    connection.Open();
                    var commandText = "";

                    commandText = @"SELECT BookId, Title, Author, PublicationDate, Quantaty
                                    FROM Books
                                    WHERE
                                        CASE
                                            WHEN @AvailableOnly
                                            THEN  Quantaty > 0
                                            ELSE True
                                        END;";
                    var result = await connection.QueryAsync<BookDBRecord>(commandText, new { AvailableOnly  = availableOnly });

                    return result.ToList();
                }
            }
            catch
            {
                return new List<BookDBRecord>();
            }
        }
    }
}
