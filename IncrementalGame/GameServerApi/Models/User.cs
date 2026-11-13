using System.ComponentModel.DataAnnotations;

namespace GameServerApi.Models;

public class User(UserPass userPass)
{
    private static int _cpt = 0;

    public int Id { get; set; } = _cpt++;

    public string Pseudo { get; } = userPass.Pseudo;
    public string Password { get; } = userPass.Password;
    public UserRole UserRole { get; set; }
}