using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using GameServerApi.Models;
using Microsoft.EntityFrameworkCore;

namespace GameServerApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GameController(BDDContext context) : ControllerBase
{
    private static int CalculateResetCost(int multiplier)
    {
        const double baseCost = 100.0;
        const double growthFactor = 1.5;
        var cost = baseCost * Math.Pow(growthFactor, multiplier - 1);
        return (int)Math.Floor(cost);
    }

    // GET /api/Game/Progression/{userId}
    [HttpGet("Progression/{userId:int}")]
    public async Task<ActionResult> GetProgression(int userId)
    {
        if (null == await context.Users.FindAsync(userId))
        {
            return NotFound(new ErrorResponse("User not found", "USER_NOT_FOUND"));
        }

        var p = await context.Progressions.FirstOrDefaultAsync(p=>p.UserId.Equals(userId));
        if (null == p)
        {
            return BadRequest(new ErrorResponse("No progression", "NO_PROGRESSION"));
        }

        return Ok(p);
    }
    
    // GET /api/Game/Initialize/{userId}
    [HttpGet("Initialize/{userId:int}")]
    public async Task<ActionResult> Initialize(int userId)
    {
        if (null == await context.Users.FindAsync(userId))
        {
            return NotFound(new ErrorResponse("User not found", "USER_NOT_FOUND"));
        }
        
        var p = await context.Progressions.FirstOrDefaultAsync(p=>p.UserId.Equals(userId));
        if (null != p)
        {
            return BadRequest(new ErrorResponse("Progression exists", "PROGRESSION_EXISTS"));
        }

        p = new Progression(userId);
        context.Progressions.Add(p);
        try
        {
            await context.SaveChangesAsync(); 
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return BadRequest(new ErrorResponse("Échec lors de l'initialisation", "INITIALIZATION_FAILED"));
        }

        return Ok(p);
    }
    
    // GET /api/Game/Click/{userId}
    [HttpGet("Click/{userId:int}")]
    public async Task<ActionResult> Click(int userId)
    {
        if (null == await context.Users.FindAsync(userId))
        {
            return NotFound(new ErrorResponse("User not found", "USER_NOT_FOUND"));
        }
        
        var p = await context.Progressions.FirstOrDefaultAsync(p=>p.UserId.Equals(userId));
        if (null == p)
        {
            return BadRequest(new ErrorResponse("No progression", "NO_PROGRESSION"));
        }

        p.Count += p.Multiplier;
        await context.SaveChangesAsync();
        return Ok(new ClickResponse(p.Count,p.Multiplier));
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
            await context.SaveChangesAsync();
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
