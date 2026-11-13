using System.ComponentModel.DataAnnotations;

namespace GameServerApi.Models;

public class User(string pseudo = "Utilisateur", string password = "")
{
    private static int _cpt = 0;

    public int Id { get; set; } = _cpt++;

    public string Pseudo { get; set; } = pseudo;
    public string Password { get; set; } = password;
    public UserRole Role { get; set; }
}