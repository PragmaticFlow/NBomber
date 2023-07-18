using Npgsql;
using Dapper;
using Newtonsoft.Json;
using WebAppSimulator.Contracts.Bookstore;
using WebAppSimulator.Contracts;

namespace WebAppSimulator.Infra.Bookstore.DAL
{
    public class UserDBRecord
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public string UserData { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime UpdatedDateTime { get; set; }

        internal static UserDBRecord Create(SingUpUserRequest request, Guid idUser, string passwordHash,
                                            byte[] passwordSalt, DateTime createdDT, DateTime updatedDT)
        {
            var userData = new UserData { FirstName = request.FirstName, LastName = request.LastName };
            return new UserDBRecord()
            {
                UserId = idUser,
                Email = request.Email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                CreatedDateTime = createdDT,
                UpdatedDateTime = updatedDT,
                UserData = JsonConvert.SerializeObject(userData)
            };
        }
    }

    public class UserLoginDBRecord
    {
        public Guid UserId { get; set; }
        public string PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
    }

        public class UserData
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class BookstoreUserRepository
    {
        private string _connectionStr;
        public BookstoreUserRepository(BookstoreSettings settings)
        {
            _connectionStr = settings.ConnectionString;
        }
        public async Task<bool> InsertUser(UserDBRecord record)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionStr))
                {
                    connection.Open();

                    var commandText = @"INSERT INTO Users 
                                    (UserId, Email, PasswordHash, PasswordSalt, UserData, CreatedDateTime, UpdatedDateTime) 
                                    VALUES (@UserId, @Email, @PasswordHash, @PasswordSalt::bytea, @UserData::jsonb, @CreatedDateTime, @UpdatedDateTime)";
                    var command = new NpgsqlCommand(commandText, connection);
                    await connection.ExecuteAsync(commandText, record);
                }
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }                   
        }

        public async Task<UserLoginDBRecord?> TryFindUserLoginData(string email)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionStr))
                {
                    connection.Open();

                    var commandText = "SELECT UserId, PasswordHash, PasswordSalt FROM Users WHERE Email = @Email";

                    var result = await connection.QueryAsync<UserLoginDBRecord>(commandText, new { Email = email });
                    return result.First();
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
