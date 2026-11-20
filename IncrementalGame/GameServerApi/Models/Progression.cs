namespace GameServerApi.Models;

public class Progression
{
    public int Id { get; init; }
    public required int UserId;
    public int Count = 0;
    public int Multiplier = 1;
    public int BestScore = 0;
}