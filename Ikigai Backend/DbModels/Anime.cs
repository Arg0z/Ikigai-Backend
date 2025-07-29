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
        public List<Episode> Episodes { get; set; } = new();
        public List<AnimeGenre> AnimeGenres { get; set; } = new();
        public List<Favourite> UserFavourites { get; set; } = new();
        public List<Following> UserFollowings { get; set; } = new();
        public List<Review> Reviews { get; set; } = new();
    }
}
