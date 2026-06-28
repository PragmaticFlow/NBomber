using System.Collections.Concurrent;
using HttpApiSimulator.Contracts;

namespace HttpApiSimulator.Infra.DAL;

public class InMemoryRepository: IUserRepository
{
    ConcurrentDictionary<int, User> _users = new();

    public ValueTask<User> GetById(int id)
    {
        _users.TryGetValue(id, out var user);
        return ValueTask.FromResult(user);
    }

    public ValueTask<bool> Update(User user)
    {
        if (user == null)
            return ValueTask.FromResult(false);

        _users[user.Id] = user;
        return ValueTask.FromResult(true);
    }

    public void CreateDB()
    {
        _users = new ConcurrentDictionary<int, User>();
    }

    public void DeleteTable()
    {
        _users.Clear();
    }
}
