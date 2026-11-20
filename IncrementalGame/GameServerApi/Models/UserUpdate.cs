namespace GameServerApi.Models;

public record UserUpdate(string Username, string? Password, UserRole Role)
{
    public string Username { get; set; } = Username;
    public string? Password { get; set; } = Password;
    public UserRole Role { get; set; } = Role;
}