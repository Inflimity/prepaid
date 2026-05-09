using Microsoft.EntityFrameworkCore;
using PrepaidApi.Models;

namespace PrepaidApi.Data
{
    public class PrepaidContext : DbContext
    {
        public PrepaidContext(DbContextOptions<PrepaidContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasIndex(u => u.PhoneNumber).IsUnique();

            modelBuilder.Entity<User>()
                .Property(u => u.Balance)
                .HasConversion(
                    ngn => (int)(ngn * 100),
                    kobo => (decimal)kobo / 100m
                )
                .HasColumnType("INT");
        }
    }
}
