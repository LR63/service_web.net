namespace GameServerApi.Models;
public class User(string username="User", string password="", UserRole role=UserRole.User)
{
    private static int _nextId = 1;
    public int Id { get; init; } = _nextId++;
    public string Username { get; set; } = username;
    public string Password { get; set; } = password;
    public UserRole Role { get; set; } = role;

}