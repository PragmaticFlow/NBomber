using Dapper.Contrib.Extensions;
using System.Data.SQLite;
using WebAppSimulator.Contracts;

namespace WebAppSimulator.Infra.DAL
{
    public class SQLiteDBRepository : IUserRepository
    {
        private SQLiteConnection _connection = null;
        
        public SQLiteDBRepository(SQLiteSettings settings)
        {
            _connection = new SQLiteConnection(settings.ConnectionString);
            _connection.Open();
        }

        public void CreateDB()
        {
            using var command = _connection.CreateCommand();

            command.CommandText = "PRAGMA journal_mode=WAL";
            command.ExecuteNonQuery();

            command.CommandText = "pragma synchronous = normar";
            command.ExecuteNonQuery();
            command.CommandText = @"CREATE TABLE IF NOT EXISTS  users 
                (Id INTEGER PRIMARY KEY,
                FirstName TEXT, 
                LastName TEXT,
                Age INTEGER)";
            command.ExecuteNonQuery();
        }

        public Task<User> GetById(int id)
        {
            return _connection.GetAsync<User>(id);
        }

        public Task<bool> Update(User user)
        {
            using var command = _connection.CreateCommand();

            command.CommandText = @"INSERT INTO users (Id, FirstName, LastName, Age)
                VALUES (@Id, @FirstName, @LastName, @Age)
                ON CONFLICT(Id)
                DO UPDATE SET FirstName = excluded.FirstName, LastName = excluded.LastName, Age = excluded.Age;";

            command.Parameters.AddWithValue("@Id", user.Id);
            command.Parameters.AddWithValue("@FirstName", user.FirstName);
            command.Parameters.AddWithValue("@LastName", user.LastName);
            command.Parameters.AddWithValue("@Age", user.Age);

            var affectedRows = command.ExecuteNonQuery();

            return Task.FromResult(affectedRows > 0);
        }

        public void DeleTable()
        {
            using var command = _connection.CreateCommand();

            command.CommandText = "DROP TABLE IF EXISTS users";
            command.ExecuteNonQuery();
        }
    }
}
