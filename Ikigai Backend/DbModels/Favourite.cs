namespace Ikigai_Backend.DbModels
{
    public class Favourite
    {
        public int UserId { get; set; }
        public User User { get; set; }
        public int AnimeId { get; set; }
        public Anime Anime { get; set; }

    }
}
