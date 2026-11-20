namespace GameServerApi.Models;
public record UserPublic(User? User)
{
    public int Id { get; } = User.Id;
    public string Pseudo { get; } = User.Username;
    public UserRole UserRole { get; } = User.Role;

}