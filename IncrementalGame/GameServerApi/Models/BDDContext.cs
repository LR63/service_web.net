using Microsoft.EntityFrameworkCore;
namespace GameServerApi.Models;

public class BDDContext(DbContextOptions<BDDContext> options) : DbContext(options)
{
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        // Connexion à la base SQLite
        options.UseSqlite("Data Source=BDD.db");
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Progression> Progressions { get; set; } = null!;
}