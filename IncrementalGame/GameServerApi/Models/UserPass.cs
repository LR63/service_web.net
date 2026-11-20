namespace GameServerApi.Models;
public record UserPass
{
    public required string Username { get; set; }
    public required string Password { get; set; }
}