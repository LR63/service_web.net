using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using GameServerApi.Models;
using Microsoft.EntityFrameworkCore;

namespace GameServerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameController(BDDContext context) : ControllerBase
    {

        private static int CalculateResetCost(int multiplier)
        {
            double baseCost = 100.0;
            double growthFactor = 1.5;
            double cost = baseCost * Math.Pow(growthFactor, multiplier - 1);
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
    }
}
