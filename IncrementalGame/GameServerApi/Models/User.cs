using System.ComponentModel.DataAnnotations;

namespace GameServerApi.Models;
public class User(string username="User", string password="", UserRole role=UserRole.User)
{
    public int Id { get; init; }
    
    [StringLength(32)]
    public string Username { get; set; } = username;
    
    [StringLength(128)]
    public string Password { get; set; } = password;
    public UserRole Role { get; set; } = role;

}