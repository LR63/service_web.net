namespace GameServerApi.Models;

public class Game
{
    public int Count = 0;
    public int Multiplier = 1;
    public int BestScore = 0;
    
    private int CalculateResetCost(int multiplier)
    {
        // Coût exponentiel : 100 * 1.5^(multiplier - 1)
        const double baseCost = 100.0;
        const double growthFactor = 1.5;
        var cost = baseCost * Math.Pow(growthFactor, multiplier - 1);
        return (int)Math.Floor(cost);
    }
}