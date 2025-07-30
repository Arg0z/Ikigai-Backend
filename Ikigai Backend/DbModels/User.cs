using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ikigai_Backend.DbModels
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        [MaxLength(256)]
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public DateTime Created_at { get; set; }
        public List<UserRole> UserRoles { get; set; } = new();
        public List<Favourite> UserFavourites { get; set; } = new();
        public List<Following> UserFollowings { get; set; } = new();
        public List<Review> Reviews { get; set; } = new();
        public List<History> Histories { get; set; } = new();
        public List<Playlist> Playlists { get; set; } = new();
    }
}
