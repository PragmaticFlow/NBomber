using System.Data.SQLite;
using Dapper.Contrib.Extensions;
using WebAppSimulator.Contracts;

namespace WebAppSimulator.Infra.DAL
{
    public class SQLiteDBRepository : IUserRepository
    {
        private SQLiteCommand _command = null;

        private SQLiteConnection _connection = null;
        
        public SQLiteDBRepository(SQLiteSettings settings)
        {
            _connection = new SQLiteConnection(settings.ConnectionString);
            _connection.Open();
            _command = new SQLiteCommand(_connection);
        }
        public void CreateDB()
        {
            _command.CommandText = "PRAGMA journal_mode=WAL";
            _command.ExecuteNonQuery();

            _command.CommandText = "pragma synchronous = normar";
            _command.ExecuteNonQuery();
            _command.CommandText = @"CREATE TABLE IF NOT EXISTS  users 
                    (Id INTEGER PRIMARY KEY,
                    FirstName TEXT, 
                    LastName TEXT,
                    Age INTEGER)";
            _command.ExecuteNonQuery();
        }

        public Task<User> GetById(int id)
        {
            return _connection.GetAsync<User>(id);
        }

        public Task Insert(User user)
        {
            _connection.Insert(user);
            return Task.CompletedTask;
        }

        public Task<bool> Update(User user)
        {
            return _connection.UpdateAsync(user);
        }
        public void DeleTable()
        {
            _command.CommandText = "DROP TABLE IF EXISTS users";
            _command.ExecuteNonQuery();
        }
    }
}
