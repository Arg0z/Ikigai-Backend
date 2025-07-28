namespace Ikigai_Backend.DTOs.AnimeDTOs
{
    public class PostAnimeDTO
    {
        public string AnimeTitle { get; set; } = string.Empty;
        public string Synopsis { get; set; } = string.Empty;
        public DateOnly ReleaseDate { get; set; }
        public DateTime UploadDate { get; set; }
        public bool IsOngoing { get; set; }
        public string Studio { get; set; } = string.Empty;
        public DateTime LastUpdate { get; set; } = DateTime.UtcNow;
    }
}
