namespace GameServerApi.Models;

public class Progression(int userId = 0)
{
    public int Id { get; init; }
    public int UserId { get; set; } = userId;
    public int Count { get; set; } = 0;
    public int Multiplier { get; set; } = 1;
    public int BestScore { get; set; } = 0;
}