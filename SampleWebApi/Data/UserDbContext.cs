using BankSystemWebApi.Model;
using Microsoft.EntityFrameworkCore;

namespace BankSystemWebApi.Data
{
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options)
            : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
    }
}
