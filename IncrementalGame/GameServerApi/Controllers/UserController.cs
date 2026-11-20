using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using GameServerApi.Models;
using Microsoft.EntityFrameworkCore;

namespace GameServerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly BDDContext _context;
        private readonly PasswordHasher<User> _hasher;

        public UserController(BDDContext context, PasswordHasher<User> hasher)
        {
            _context = context;
            _hasher = hasher;
        }

        // GET /api/User/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<UserPublic>> GetUser(int id)
        {
            var u = await _context.Users.FindAsync(id);

            if (null != u)
            {
                return Ok(new UserPublic(u));
            }
            return NotFound(new ErrorResponse("User not found", "USER_NOT_FOUND"));
        }

        // GET /api/User/All
        [HttpGet("All")]
        public async Task<ActionResult<List<UserPublic>>> GetAll()
        {
            return Ok(await _context.Users.Select(u => new UserPublic(u)).ToListAsync());
        }

        // POST /api/User/Register
        [HttpPost("Register")]
        public async Task<ActionResult<UserPublic>> Register(UserPass upass)
        {
            if (await _context.Users.AnyAsync(x => x.Username.Equals(upass.Username)))
            {
                return Conflict(new ErrorResponse("Username already exists", "USERNAME_EXISTS"));
            }
            User user = new(upass.Username);
            user.Password = _hasher.HashPassword(user, upass.Password);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return Ok(new UserPublic(user));
        }

        // POST /api/User/Login
        [HttpPost("Login")]
        public async Task<ActionResult<UserPublic>> Login(UserPass upass)
        {
            User? user = await _context.Users.FirstOrDefaultAsync(x => x.Username.Equals(upass.Username));

            if (null != user)
            {
                var result = _hasher.VerifyHashedPassword(user, user.Password, upass.Password);
                if (result != PasswordVerificationResult.Failed)
                {
                    return Ok(new UserPublic(user));
                }
                return StatusCode(401, new ErrorResponse("Incorrect password", "INVALID_PASSWORD"));
            }
            return NotFound(new ErrorResponse("User not found", "USER_NOT_FOUND"));
        }

        // PUT /api/User/{id}
        [HttpPut("{id:int}")]
        public async Task<ActionResult<UserPublic>> Update(int id, UserUpdate update)
        {
            User? u = await _context.Users.FindAsync(id);

            if (null != u)
            {
                u.Username = update.Username;
                u.Role = update.Role;

                if (!string.IsNullOrWhiteSpace(update.Password))
                    u.Password = _hasher.HashPassword(u, update.Password);
                
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    return StatusCode(409, new ErrorResponse("Concurrency error", "CONCURRENCY_ERROR"));
                }

                return Ok(new UserPublic(u));
            }
            return NotFound(new ErrorResponse("User not found", "USER_NOT_FOUND"));
        }

        // DELETE /api/User/{id}
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            User? u = await _context.Users.FindAsync(id);
            if (null == u)
                return NotFound(new ErrorResponse("User not found", "USER_NOT_FOUND"));

            _context.Users.Remove(u);
            await _context.SaveChangesAsync();

            return Ok(true);
        }

        // GET /api/User/AllAdmin
        [HttpGet("AllAdmin")]
        public async Task<ActionResult<List<UserPublic>>> AllAdmin()
        {
            return Ok(await _context.Users.Where(u => UserRole.Admin == u.Role).Select(u => new UserPublic(u)).ToListAsync());
        }

        // GET /api/User/Search/{name}
        [HttpGet("Search/{name}")]
        public async Task<ActionResult<List<UserPublic>>> Search(string name)
        {
            return Ok(await _context.Users.Where(u => u.Username.Contains(name)).Select(u => new UserPublic(u)).ToListAsync());
        }
    }
}
