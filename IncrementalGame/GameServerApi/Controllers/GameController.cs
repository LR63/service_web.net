using Microsoft.AspNetCore.Identity;
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
            return BadRequest(new ErrorResponse("No progression", "NO_PROGRESSION"));
        }

        return Ok(p);
    }

    private static int CalculateResetCost(int multiplier)
    {
        const double baseCost = 100.0;
        const double growthFactor = 1.5;
        var cost = baseCost * Math.Pow(growthFactor, multiplier - 1);
        return (int)Math.Floor(cost);
    }


    // GET /api/Game/ResetCost/{userId}
    [HttpGet("ResetCost/{id:int}")]
    public async Task<ActionResult<ResetCostResponse>> GetRestCost(int id)
    {
        var userProgression = await context.Progressions.FindAsync(id);
        if (null == userProgression)
        {
            return BadRequest(new ErrorResponse("User has no progression", "NO_PROGRESSION"));
        }
        return Ok(new ResetCostResponse(CalculateResetCost(userProgression.Multiplier)));
    }

    // POST /api/Game/Reset/{userId}
    [HttpPost("Reset/{id:int}")]
    public async Task<ActionResult<Progression>> Reset(int id)
    {
        var userProgression = await context.Progressions.FindAsync(id);
        if (null == userProgression)
        {
            return BadRequest(new ErrorResponse("User has no progression", "NO_PROGRESSION"));
        }
        int resetCost = CalculateResetCost(userProgression.Multiplier);
        if (userProgression.Count >= resetCost)
        {
            userProgression.BestScore = Math.Max(userProgression.BestScore, userProgression.Count);
            userProgression.Count = 0;
            userProgression.Multiplier += 1;
        }
        else
        {
            return BadRequest(new ErrorResponse("Not enough points to reset", "INSUFFICIENT_CLICKS"));
        }
        return Ok(userProgression);
    }

    // GET /api/Game/BestScore
    [HttpGet("BestScore")]
    public async Task<ActionResult<BestScoreResponse>> GetBestScore()
    {
        if (!await context.Progressions.AnyAsync())
        {
            return NotFound(new ErrorResponse("No progressions found", "NO_PROGRESSIONS"));
        }
        var bestUser = await context.Progressions.OrderByDescending(p => p.BestScore).FirstAsync();
        var bestScore = bestUser.BestScore;
        return Ok(new BestScoreResponse(bestUser.UserId, bestScore));
    }
}
