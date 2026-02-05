using Chris.Mongodb.Demo.Entities;
using Chris.Mongodb.Demo.Services;
using Microsoft.AspNetCore.Mvc;

namespace Chris.Mongodb.Demo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        // POST: api/User
        [HttpPost]
        public async Task<ActionResult<User>> AddUser(User user)
        {
            var exsitedUser = await _userService.GetUserByIdAsync(user.Id);
            if (exsitedUser != null)
            {
                return Conflict("The user is already existed");
            }

            var newUser = await _userService.AddUserAsync(user);
            return CreatedAtAction(nameof(GetUserById), new { id = newUser.Id }, newUser);
        }

        // GET: api/User
        [HttpGet]
        public async Task<ActionResult<List<User>>> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        // GET: api/User/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUserById(string id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound("The user does not exist");
            }
            return Ok(user);
        }

        // PUT: api/User/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest("ID mismatch");
            }
            var isUpdated = await _userService.UpdateUserAsync(id, user);
            if (!isUpdated)
            {
                return NotFound("");
            }
            return NoContent();
        }

        // DELETE: api/User/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var isDeleted = await _userService.DeleteUserAsync(id);
            if (!isDeleted)
            {
                return NotFound("The user does not exist");
            }
            return NoContent();
        }
    }
}
