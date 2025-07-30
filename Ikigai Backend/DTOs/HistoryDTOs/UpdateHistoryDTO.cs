namespace Ikigai_Backend.DTOs.HistoryDTOs
{
    public class UpdateHistoryDTO
    {
        public int UserId { get; set; }
        public int EpisodeID { get; set; }
        public DateTime WatchedAt { get; set; }
    }
}
