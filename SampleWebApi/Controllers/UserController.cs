using BankSystemWebApi.Model;
using BankSystemWebApi.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace BankSystemWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        //Create a variable to hold the instance of the IUserRepository
        private readonly IUserRepository _userRepositoryFactory;
        /// <summary>
        /// Initializing the _userRepositoryFactory 
        /// </summary>
        /// <param name="userRepositoryFactory"></param>
        public UserController(IUserRepository userRepositoryFactory)
        {
            _userRepositoryFactory = userRepositoryFactory;
        }
        /// <summary>
        /// Get user by Id to get the user balance
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/BalanceInquiry")]
        [ProducesResponseType(typeof(decimal), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetBalance(int id)
        {
            //Call the GetUserByIdAsync method of the IUserRepository to find user by Id.
            var user = await _userRepositoryFactory.GetUserByIdAsync(id);
            return user == null ? NotFound() : Ok(user.Balance);
        }
        /// <summary>
        /// This action method will add new user.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddNewUser(User user)
        {
            //Call the InsertUser method of IUserRepository to insert the new user in the User Table
            var users = await _userRepositoryFactory.InsertUser(user);
            if (user == null) return BadRequest();
            //Save Changes made in the user table
            await _userRepositoryFactory.SaveChangesAsync();
            return CreatedAtAction(nameof(AddNewUser), users);
        }
        /// <summary>
        /// This action method will add balance in the user's balance in the user table
        /// </summary>
        /// <param name="id"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        [HttpPut("{id}/{amount}/Deposit")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Deposit(int id, decimal amount)
        {
            //Get user by id of IUserRepository and check if the user exist or not
            var user = await _userRepositoryFactory.GetUserByIdAsync(id);
            if (user == null) return NotFound();
            //If the user is valid, will call the AddBalance method of IUserRepository to update the balance of the user
            var updatedUser = _userRepositoryFactory.AddBalance(user, amount);
            if (updatedUser == null) return BadRequest();
            //Save Changes made in the user table
            await _userRepositoryFactory.SaveChangesAsync();
            return NoContent();
        }
        /// <summary>
        /// This action method will substact balance in the user's balance in the user table
        /// </summary>
        /// <param name="id"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        [HttpPut("{id}/{amount}/WithdrawCash")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> WithdrawCash(int id, decimal amount)
        {
            //Get and check if the user exist in the user table
            var user = await _userRepositoryFactory.GetUserByIdAsync(id);
            if (user == null) return NotFound();
            //Check if the user has valid balance for the transaction
            if (amount > user.Balance) return Conflict();
            var updatedUserBal = _userRepositoryFactory.SubtractBalance(user, amount);
            if (updatedUserBal == null) return BadRequest();
            //Save Changes made in the user table
            await _userRepositoryFactory.SaveChangesAsync();
            return NoContent();
        }
        /// <summary>
        /// This action method will transfer money from one uset to another in user table
        /// </summary>
        /// <param name="idFrom"></param>
        /// <param name="idTo"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        [HttpPut("{idFrom}/{idTo}/{amount}/TransferMoney")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> TransferMoney(int idFrom, int idTo, decimal amount)
        {
            //Get user by id of IUserRepository and check if the users exist or not
            var userFrom = await _userRepositoryFactory.GetUserByIdAsync(idFrom);
            if (userFrom == null) return NotFound();
            var userTo = await _userRepositoryFactory.GetUserByIdAsync(idTo);
            if (userTo == null) return NotFound();
            // Check if the balance of the userFrom has valid balance for transfer transaction
            if (amount > userFrom.Balance) return Conflict();
            //Call teh method SubtractBalance to remove x amount to the userFrom balnce.
            var userSubsBal = _userRepositoryFactory.SubtractBalance(userFrom, amount);
            if (userSubsBal == null) return BadRequest();
            //Call the Deposit Method of the IUserRepository to deposit money to userTo
            var userAddBal = _userRepositoryFactory.AddBalance(userTo, amount);
            if (userAddBal == null) return BadRequest();
            //Save Changes made in the user table
            await _userRepositoryFactory.SaveChangesAsync();
            return NoContent();
        }
    }
}
