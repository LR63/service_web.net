using System.ComponentModel.DataAnnotations;

namespace GameServerApi.Models;

public class User(UserInfo userInfo)
{
    private static int _cpt = 0;

    public int Id { get; set; } = _cpt++;

    public string Pseudo { get; } = userInfo.Pseudo;
    public string Password { get; } = userInfo.Password;
    public Role Role { get; set; }
}