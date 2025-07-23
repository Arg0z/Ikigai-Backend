using Ikigai_Backend.DbModels;
using Microsoft.EntityFrameworkCore;
using System;

namespace Ikigai_Backend.Database
{
    public class IkigaiDbContext : DbContext
    {
        public IkigaiDbContext(DbContextOptions<IkigaiDbContext> options)
       : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.UserId, ur.RoleName });

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId);

            // Ensure email is unique
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }
    }
}
