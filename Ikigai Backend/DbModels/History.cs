namespace Ikigai_Backend.DbModels
{
    public class History
    {
        public int UserId { get; set; }
        public User User { get; set; }
        public int EpisodeID { get; set; }
        public Episode Episode { get; set; }
        public DateTime WatchedAt { get; set; }
    }
}
