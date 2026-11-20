namespace GameServerApi.Models;
public class UserPublic(User? user)
{
    public int Id { get; } = user.Id;
    public string Pseudo { get; } = user.Username;
    public UserRole UserRole { get; } = user.Role;

}