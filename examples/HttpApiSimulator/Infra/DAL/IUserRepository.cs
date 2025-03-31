using WebAppSimulator.Contracts;

namespace WebAppSimulator.Infra.DAL
{
    public interface IUserRepository
    {
        Task<User> GetById(int id);
        Task Insert(User user);
        Task<bool> Update(User user);
        void CreateDB();
        void DeleTable();
    }
}
