using HttpApiSimulator.Contracts;

namespace HttpApiSimulator.Infra.DAL;

public interface IUserRepository
{
    ValueTask<User> GetById(int id);
    ValueTask<bool> Update(User user);
    void CreateDB();
    void DeleTable();
}
