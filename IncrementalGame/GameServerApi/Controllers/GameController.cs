using Microsoft.AspNetCore.Mvc;
using GameServerApi.Models;
using Microsoft.EntityFrameworkCore;

namespace GameServerApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GameController(BDDContext context) : ControllerBase
{
    [HttpGet("Progression/{userId:int}")]
    public async Task<ActionResult> GetProgression(int userId)
    {
        var u = await context.Users.FindAsync(userId);

        if (null == u)
        {
            return NotFound(new ErrorResponse("User not found", "USER_NOT_FOUND"));
        }
        var p = await context.Progressions.FindAsync(u.Id);
        if (null == p)
        {
            return BadRequest(new ErrorResponse("No progression","NO_PROGRESSION"));
        }

        return Ok(p);
    }
}
