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
        public OrderRepository(BookstoreSettings settings)
        {
            _connectionStr = settings.ConnectionString;
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
                        var commandUpdate = "UPDATE Books SET Quantaty = Quantaty - 1 WHERE BookId = @BookId";
                        await connection.ExecuteAsync(commandUpdate, new { request.BookId }, transaction);

                        var commandInsert = @"INSERT INTO Orders 
                                    (UserId, BookId, Quantaty) 
                                    VALUES (@UserId, @BookId, @Quantaty)";
                        await connection.ExecuteAsync(commandInsert, request, transaction);
                        transaction.Commit();
                        return true;
                    }
                    catch
                    {
                        transaction.Rollback();
                        return false;
                    }
                }   
            }
        }
    }
}
