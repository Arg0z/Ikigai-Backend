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
        public DbSet<Favourite> UserFavourites { get; set; }
        public DbSet<Following> UserFollowings { get; set; }
        public DbSet<EpisodeVideo> EpisodeVideo { get; set; } = default!;
        public DbSet<EpisodeAudio> EpisodeAudio { get; set; } = default!;
        public DbSet<EpisodeSub> EpisodeSub { get; set; } = default!;
        public DbSet<Genre> Genres { get; set; } = default!;         // Add this line
        public DbSet<AnimeGenre> AnimeGenres { get; set; } = default!; // Add this line
        public DbSet<Review> Reviews { get; set; } = default!;

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

            // Composite key and foreign keys for Favourite
            modelBuilder.Entity<Favourite>()
                .HasKey(f => new { f.UserId, f.AnimeId });

            modelBuilder.Entity<Favourite>()
                .HasOne(f => f.User)
                .WithMany()
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Favourite>()
                .HasOne(f => f.Anime)
                .WithMany()
                .HasForeignKey(f => f.AnimeId)
                .OnDelete(DeleteBehavior.Cascade);

            // Composite key and foreign keys for Following
            modelBuilder.Entity<Following>()
                .HasKey(f => new { f.UserId, f.AnimeId });

            modelBuilder.Entity<Following>()
                .HasOne(f => f.User)
                .WithMany()
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Following>()
                .HasOne(f => f.Anime)
                .WithMany()
                .HasForeignKey(f => f.AnimeId)
                .OnDelete(DeleteBehavior.Cascade);


            // Ensure user's email is unique
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // AnimeGenre composite key and relationships
            modelBuilder.Entity<AnimeGenre>()
                .HasKey(ag => new { ag.AnimeId, ag.GenreId });

            modelBuilder.Entity<AnimeGenre>()
                .HasOne(ag => ag.Anime)
                .WithMany(a => a.AnimeGenres)
                .HasForeignKey(ag => ag.AnimeId);

            modelBuilder.Entity<AnimeGenre>()
                .HasOne(ag => ag.Genre)
                .WithMany(g => g.AnimeGenres)
                .HasForeignKey(ag => ag.GenreId);

            // Review relationships
            modelBuilder.Entity<Review>()
                .HasKey(r => new { r.UserId, r.AnimeId });

            modelBuilder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Anime)
                .WithMany(a => a.Reviews)
                .HasForeignKey(r => r.AnimeId)
                .OnDelete(DeleteBehavior.Cascade);

            // ... rest of your configuration ...
        }
    }
}
