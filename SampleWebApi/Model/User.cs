using System.ComponentModel.DataAnnotations;

namespace BankSystemWebApi.Model
{
    public class User
    {
        public int UserId { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
        public decimal Balance { get; set; }
        public DateTime Created { get; set; }
    }
}
