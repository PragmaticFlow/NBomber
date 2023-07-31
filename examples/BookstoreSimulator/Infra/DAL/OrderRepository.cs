using BookstoreSimulator.Contracts;
using Dapper;
using Npgsql;

namespace BookstoreSimulator.Infra.DAL
{
    public class OrderDBRecord
    {
        public Guid UserId { get; private set; }
        public Guid BookId { get; private set; }
        public int Quantaty { get; private set; }

        public OrderDBRecord(OrderRequest request, Guid userId)
        {
            UserId = userId;
            BookId = request.BookId;
            Quantaty = request.Quantaty;
        }
    }

    public class OrderRepository
    {
        private string _connectionStr;
        private Serilog.ILogger _logger;

        public OrderRepository(BookstoreSettings settings, Serilog.ILogger logger)
        {
            _connectionStr = settings.ConnectionString;
            _logger = logger;
        }

        public async Task<bool> CreateOrder(OrderDBRecord request)
        {
            using (var connection = new NpgsqlConnection(_connectionStr))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
                {
                    try
                    {
                        var commandUpdate = @"UPDATE Books SET Quantaty = Quantaty - 1 WHERE BookId = @BookId";        
                        await connection.ExecuteAsync(commandUpdate, new { request.BookId }, transaction);

                        var commandInsert = @"INSERT INTO Orders 
                                    (UserId, BookId, Quantaty) 
                                    VALUES (@UserId, @BookId, @Quantaty)";
                        await connection.ExecuteAsync(commandInsert, request, transaction);
                        transaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex, "Create order DB error");
                        transaction.Rollback();
                        return false;
                    }
                }   
            }
        }
    }
}
