namespace GameServerApi.Models;
using System.ComponentModel.DataAnnotations;

public record UserPass
{
    [Required]
    [MinLength(3)]
    [MaxLength(20)]
    [RegularExpression(@"^[a-zA-Z0-9]+$")]
    public required string Username { get; set; }

    [Required]
    [MinLength(4)]
    [MaxLength(20)]
    [RegularExpression(@"^[a-zA-Z0-9&^!@#]+$")]
    public required string Password { get; set; }
}