namespace Ikigai_Backend.DbModels
{
    public class Review
    {
        public int UserId { get; set; }
        public User User { get; set; } = default!;
        public int AnimeId { get; set; }
        public Anime Anime { get; set; } = default!;
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
