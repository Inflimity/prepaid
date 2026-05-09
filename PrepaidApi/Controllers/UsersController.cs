using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrepaidApi.Data;
using PrepaidApi.Models;
using PrepaidApi.DTOs;
using PrepaidApi.Attributes;

namespace PrepaidApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiKey]
    public class UsersController : ControllerBase
    {
        private readonly PrepaidContext _context;

        public UsersController(PrepaidContext context)
        {
            _context = context;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<User>>>> GetUsers()
        {
            var users = await _context.Users.ToListAsync();
            return Ok(ApiResponse<IEnumerable<User>>.Success(users, "Users retrieved successfully"));
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<User>>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound(ApiResponse<User>.Error("User not found"));
            }

            return Ok(ApiResponse<User>.Success(user, "User retrieved successfully"));
        }

        // POST: api/Users
        [HttpPost]
        public async Task<ActionResult<ApiResponse<User>>> CreateUser([FromBody] CreateUserDto userDto)
        {
            if (!ModelState.IsValid) return BadRequest(ApiResponse<object>.Error("Invalid input"));

            var user = new User
            {
                FullName = userDto.FullName,
                PhoneNumber = userDto.PhoneNumber,
                Balance = userDto.Balance,
                LastUpdated = DateTime.UtcNow // System-generated
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, ApiResponse<User>.Success(user, "User created successfully"));
        }

        // PATCH: api/Users/5/increase
        [HttpPatch("{id}/increase")]
        public async Task<ActionResult<ApiResponse<User>>> IncreaseBalance(int id, [FromBody] UpdateBalanceDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ApiResponse<object>.Error("Invalid input"));

            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound(ApiResponse<User>.Error("User not found"));

            user.Balance += dto.Amount;
            user.LastUpdated = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(ApiResponse<User>.Success(user, "Balance increased successfully"));
        }

        // PATCH: api/Users/5/decrease
        [HttpPatch("{id}/decrease")]
        public async Task<ActionResult<ApiResponse<User>>> DecreaseBalance(int id, [FromBody] UpdateBalanceDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ApiResponse<object>.Error("Invalid input"));

            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound(ApiResponse<User>.Error("User not found"));

            if (user.Balance < dto.Amount) return BadRequest(ApiResponse<User>.Error("Insufficient balance"));

            user.Balance -= dto.Amount;
            user.LastUpdated = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(ApiResponse<User>.Success(user, "Balance decreased successfully"));
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object?>>> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound(ApiResponse<object?>.Error("User not found"));
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse<object?>.Success(null, "User deleted successfully"));
        }
    }
}
