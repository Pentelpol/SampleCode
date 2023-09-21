using BankSystemWebApi.Model;

namespace BankSystemWebApi.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(int id);
        Task<User?> InsertUser(User user);
        User? SubtractBalance(User user, decimal amount);
        User? AddBalance(User user, decimal amount);
        Task SaveChangesAsync();
    }
}
