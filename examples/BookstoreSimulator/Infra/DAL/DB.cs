using Npgsql;

namespace BookstoreSimulator.Infra.DAL
{
    public class DB
    {
        private string _connectionStr;
        private Serilog.ILogger _logger;

        public DB(BookstoreSettings settings, Serilog.ILogger logger)
        {
            _connectionStr = settings.ConnectionString;
            _logger = logger;
        }

        internal void CreateTables()
        {
            CreateUsersTable();
            CreateBooksTable();
            CreateOrdersTable();
        }

        internal void CleanTables()
        {
            CleanOrdersTables();
            CleanBooksTables();
            CleanUsersTables();          
        }

        private void CleanOrdersTables()
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionStr))
                {
                    connection.Open();

                    var commandText = @"DROP TABLE IF EXISTS Orders";

                    var command = new NpgsqlCommand(commandText, connection);
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex,"Clean orders DB error");
            }

        }

        private void CleanBooksTables()
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionStr))
                {
                    connection.Open();

                    var commandText = @"DROP TABLE IF EXISTS Books";

                    var command = new NpgsqlCommand(commandText, connection);
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Clean books DB error");
            }
        }

        private void CleanUsersTables()
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionStr))
                {
                    connection.Open();

                    var commandText = @"DROP TABLE IF EXISTS Users";

                    var command = new NpgsqlCommand(commandText, connection);
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Clean users DB error");
            }
        }

        private void CreateUsersTable()
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionStr))
                {
                    connection.Open();

                    var commandText = @"CREATE TABLE IF NOT EXISTS Users
                    (
                        UserId uuid NOT NULL,
                        Email text NOT NULL,
                        PasswordHash text NOT NULL,
                        PasswordSalt bytea NOT NULL,
                        UserData jsonb NOT NULL,
                        CreatedDateTime timestamp with time zone NOT NULL,
                        UpdatedDateTime timestamp with time zone NOT NULL,
                        CONSTRAINT EmailIndex PRIMARY KEY (Email)
                    )";

                    var command = new NpgsqlCommand(commandText, connection);
                    command.ExecuteNonQuery();
                }
            }
            catch(Exception ex)
            {
                _logger.Error(ex, "Create users DB error");
            }
        }

        private void CreateBooksTable()
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionStr))
                {
                    connection.Open();

                    var commandText = @"CREATE TABLE IF NOT EXISTS Books
                    (
                        BookId uuid NOT NULL,
                        Title text NOT NULL,
                        Author text NOT NULL,
                        PublicationDate timestamp with time zone NOT NULL,
                        Quantaty integer NOT NULL
                    )";

                    var command = new NpgsqlCommand(commandText, connection);
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Create books DB error");
            }
        }

        private void CreateOrdersTable()
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionStr))
                {
                    connection.Open();

                    var commandText = @"CREATE TABLE IF NOT EXISTS Orders
                    (
                        OrderNumber integer PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
                        UserId uuid NOT NULL,
                        BookId uuid NOT NULL,
                        Quantaty integer NOT NULL
                    )";

                    var command = new NpgsqlCommand(commandText, connection);
                    command.ExecuteNonQuery();
                }
            }
            catch( Exception ex)
            {
                _logger.Error(ex, "Create orders DB error");
            }
        }
    }
}
