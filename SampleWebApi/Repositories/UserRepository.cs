using BankSystemWebApi.Data;
using BankSystemWebApi.Model;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace BankSystemWebApi.Repositories
{
    public class UserRepository : IUserRepository
    {
        //Create a variable to hold the instance of the UserDbContext
        private readonly UserDbContext context;
        private readonly ILogger<UserRepository> _logger;
        /// <summary>
        /// Initialzing the UserDbContext instance
        /// </summary>
        /// <param name="context"></param>
        /// <param name="_logger"></param>
        public UserRepository(
            UserDbContext context, ILogger<UserRepository> _logger)
        {
            this.context = context;
            this._logger = _logger;
        }
        /// <summary>
        /// This method will Add one User object into the User table
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<User?> InsertUser(User user)
        {
            try
            { 
                user.UserId = 0;
                user.Created = DateTime.UtcNow;
                //Set the state of the entity as Added State
                await context.Users.AddAsync(user);
                return user;  
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to Add User {data}", JsonConvert.SerializeObject(user));
                return null;
            }
        }
        /// <summary>
        /// This method will add balance one User object from the User table
        /// </summary>
        /// <param name="user"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public User? AddBalance(User user, decimal amount)
        {
            try
            {
                //add the amount in the user balance
                user.Balance += amount;
                //mark the Entity State as Modified
                context.Entry(user).State = EntityState.Modified;
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to Update User {data}", JsonConvert.SerializeObject(user));
                return null;
            }
        }
        /// <summary>
        /// This method will return all user from the User table
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await context.Users.ToListAsync();
        }
        /// <summary>
        /// This method will return one user's information from the User table
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await context.Users.FindAsync(id);
        }
        /// <summary>
        /// This method will subtract a value of amount in the user balance from the database
        /// </summary>
        /// <param name="user"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public User? SubtractBalance(User user, decimal amount)
        {
            try
            {
                //subtract the amount in the user balance
                user.Balance -= amount;
                //mark the Entity State as Modified
                context.Entry(user).State = EntityState.Modified;
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update User {data}", JsonConvert.SerializeObject(user));
                return null;
            }
        }
        /// <summary>
        /// This method will make the changes permanent in the database
        /// </summary>
        /// <returns></returns>
        public async Task SaveChangesAsync()
        {
            try
            {
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update User Table");
            }
        }
    }
}
