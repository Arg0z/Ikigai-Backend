namespace Ikigai_Backend.DbModels
{
    public class Following
    {
        public int UserId { get; set; }
        public User User { get; set; }
        public int AnimeId { get; set; }
        public Anime Anime { get; set; }
    }
}
