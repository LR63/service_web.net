using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using GameServerApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using SQLitePCL;

namespace GameServerApi.Controllers;
[Route("api/[controller]")]
[ApiController]
public class UserController(BDDContext context, PasswordHasher<User> hasher) : ControllerBase
{

    // GET /api/User/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserPublic>> GetUser(int id)
    {
        var u = await context.Users.FindAsync(id);

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
        return Ok(await context.Users.Select(u => new UserPublic(u)).ToListAsync());
    }

        // POST /api/User/Register
        [HttpPost("Register")]
        public async Task<ActionResult<UserPublic>> Register(UserPass upass)
        {   
            var userfound = await context.Users.FirstOrDefaultAsync(u => upass.Username.Equals(u.Username));

            if(null == userfound) 
            {
                User user = new(upass.Username);

                if (!await context.Users.Where(u => UserRole.Admin == u.Role).AnyAsync())
                    user.Role = UserRole.Admin;
                user.Password = hasher.HashPassword(user, upass.Password);
                context.Users.Add(user);

                try
                {
                   await context.SaveChangesAsync(); 
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return BadRequest(new ErrorResponse("Échec lors de l'inscription", "REGISTRATION_FAILED"));
                }

                return Ok(new UserPublic(user));
            }
            return BadRequest(new ErrorResponse("Nom d'utilisateur déjà pris", "USERNAME_EXISTS"));
        }

    // POST /api/User/Login
    [HttpPost("Login")]
    public async Task<ActionResult<UserPublic>> Login(UserPass upass)
    {
        User? user = await context.Users.FirstOrDefaultAsync(x => x.Username.Equals(upass.Username));

        if (null != user)
        {
            var result = hasher.VerifyHashedPassword(user, user.Password, upass.Password);
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
        User? u = await context.Users.FindAsync(id);

        if (null != u)
        {
            u.Username = update.Username;
            u.Role = update.Role;

            if (!string.IsNullOrWhiteSpace(update.Password))
                u.Password = hasher.HashPassword(u, update.Password);
            
            try
            {
                await context.SaveChangesAsync();
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
        User? u = await context.Users.FindAsync(id);
        if (null == u)
            return NotFound(new ErrorResponse("User not found", "USER_NOT_FOUND"));

        context.Users.Remove(u);
        await context.SaveChangesAsync();

        Progression? uprog = await context.Progressions.FirstOrDefaultAsync(p => id == p.UserId);
        
        if (null != uprog)
            context.Progressions.Remove(uprog);
        context.Users.Remove(u);
        await context.SaveChangesAsync();
        return Ok(true);
    }

    // GET /api/User/AllAdmin
    [HttpGet("AllAdmin")]
    public async Task<ActionResult<List<UserPublic>>> AllAdmin()
    {
        return Ok(await context.Users.Where(u => UserRole.Admin == u.Role).Select(u => new UserPublic(u)).ToListAsync());
    }

    // GET /api/User/Search/{name}
    [HttpGet("Search/{name}")]
    public async Task<ActionResult<List<UserPublic>>> Search(string name)
    {
        return Ok(await context.Users.Where(u => u.Username.Contains(name)).Select(u => new UserPublic(u)).ToListAsync());
    }
}
