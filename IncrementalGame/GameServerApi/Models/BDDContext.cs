using Microsoft.EntityFrameworkCore;
namespace GameServerApi.Models;

public class BDDContext(DbContextOptions<BDDContext> options) : DbContext(options)
    {
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite("Data Source=BDD.db");
        }
        public DbSet<User> Users { get; set; }
    }
