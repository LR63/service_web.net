namespace GameServerApi.Models
{
    public record BestScoreResponse(int UserId, int BestScore)
    {
        public int UserId { get; set; } = UserId;
        public int BestScore { get; set; } = BestScore;
    }
}
