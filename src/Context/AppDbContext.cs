using Microsoft.EntityFrameworkCore;
using Profile.Models;

namespace Profile.Context
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseNpgsql("Host=localhost;Port=5432;Pooling=true;Database=Profiles;User Id=postgres;Password=1234;");
    
    }
}