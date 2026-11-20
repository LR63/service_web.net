namespace GameServerApi.Models;

public class UserUpdate(string username, string? password, UserRole role)
{
    public string Username { get; set; } = username;
    public string? Password { get; set; } = password;
    public UserRole Role { get; set; } = role;
}