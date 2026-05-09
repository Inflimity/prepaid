using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrepaidApi.Data;
using PrepaidApi.Models;

namespace PrepaidApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly PrepaidContext _context;

        public UsersController(PrepaidContext context)
        {
            _context = context;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // POST: api/Users
        [HttpPost]
        public async Task<ActionResult<User>> CreateUser(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }

        // PATCH: api/Users/5/increase
        [HttpPatch("{id}/increase")]
        public async Task<IActionResult> IncreaseBalance(int id, [FromBody] decimal amount)
        {
            if (amount <= 0) return BadRequest("Amount must be positive.");

            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            user.Balance += amount;
            user.LastUpdated = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(user);
        }

        // PATCH: api/Users/5/decrease
        [HttpPatch("{id}/decrease")]
        public async Task<IActionResult> DecreaseBalance(int id, [FromBody] decimal amount)
        {
            if (amount <= 0) return BadRequest("Amount must be positive.");

            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            if (user.Balance < amount) return BadRequest("Insufficient balance.");

            user.Balance -= amount;
            user.LastUpdated = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(user);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
