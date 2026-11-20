namespace GameServerApi.Models
{
    public record ErrorResponse(string Message, string Code)
    {
        public string Message { get; } = Message;
        public string Code { get; } = Code;
    }
}
