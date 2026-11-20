namespace GameServerApi.Models;
using System.ComponentModel.DataAnnotations;

public record UserUpdate(string Username, string? Password, UserRole Role)
{
    [Required]
    [MinLength(3)]
    [MaxLength(20)]
    [RegularExpression(@"^[a-zA-Z0-9]+$")]
    public string Username { get; set; } = Username;

    [MaxLength(20)]
    [RegularExpression(@"^[a-zA-Z0-9]+${4,20}")]
    public string? Password { get; set; } = Password;
    public UserRole Role { get; set; } = Role;
}