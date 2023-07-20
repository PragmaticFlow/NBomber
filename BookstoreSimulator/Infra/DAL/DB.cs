using Npgsql;

namespace BookstoreSimulator.Infra.DAL
{
    public class DB
    {
        private string _connectionStr;
        public DB(BookstoreSettings settings)
        {
            _connectionStr = settings.ConnectionString;
        }

        internal void CreateTables()
        {
            CreateUsersTable();
            CreateBooksTable();
        }

        private void CreateUsersTable()
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

        private void CreateBooksTable()
        {

        }
    }
}
