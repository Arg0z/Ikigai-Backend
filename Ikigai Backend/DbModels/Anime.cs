namespace Ikigai_Backend.DbModels
{
    public class Anime
    {
        public int Id { get; set; }
        public string AnimeTitle { get; set; } = string.Empty;
        public string Synopsis { get; set; } = string.Empty;
        public DateOnly ReleaseDate { get; set; }
        public DateTime UploadDate { get; set; }
        public bool IsOngoing { get; set; }
        public string Studio { get; set; } = string.Empty;
        public DateTime LastUpdate { get; set; }
        public string ImageUrl { get; set; } = string.Empty; // <-- Added
        public List<Episode> Episodes { get; set; } = new();
        public List<AnimePlaylist> AnimePlaylists { get; set; } = new();
        public List<AnimeGenre> AnimeGenres { get; set; } = new();
        public List<Review> Reviews { get; set; } = new();
        public List<Following> Followings { get; set; } = new();
        public List<Favourite> Favourites { get; set; } = new();
    }
}
