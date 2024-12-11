using Domain.Entities.Auth;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    public class CloudDbContext : DbContext
    {
        public DbSet<UserEntity> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=CatCloud;Username=postgres;Password=admin;IncludeErrorDetail=true");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
