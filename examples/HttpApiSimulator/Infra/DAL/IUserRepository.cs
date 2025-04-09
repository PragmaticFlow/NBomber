using WebAppSimulator.Contracts;

namespace WebAppSimulator.Infra.DAL
{
    public interface IUserRepository
    {
        Task<User> GetById(int id);
        Task<bool> Update(User user);
        void CreateDB();
        void DeleTable();
    }
}
