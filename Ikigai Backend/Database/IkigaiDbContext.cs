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
        public DbSet<Anime> Animes { get; set; }
        public DbSet<Episode> Episodes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.UserId, ur.RoleName });

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId);

            modelBuilder.Entity<Episode>()
                .HasOne(e => e.Anime)
                .WithMany(a => a.Episodes)
                .HasForeignKey(e => e.AnimeId);

            modelBuilder.Entity<EpisodeVideo>()
                .HasOne(ev => ev.Episode)
                .WithMany(e => e.EpisodeVideos)
                .HasForeignKey(ev => ev.EpisodeId);

            modelBuilder.Entity<EpisodeAudio>()
                .HasOne(ev => ev.Episode)
                .WithMany(e => e.EpisodeAudios)
                .HasForeignKey(ev => ev.EpisodeId);

            modelBuilder.Entity<EpisodeSub>()
                .HasOne(es => es.Episode)
                .WithMany(e => e.EpisodeSubtitles)
                .HasForeignKey(es => es.EpisodeId);

            // Ensure user's email is unique
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }
        public DbSet<Ikigai_Backend.DbModels.EpisodeVideo> EpisodeVideo { get; set; } = default!;
    }
}
