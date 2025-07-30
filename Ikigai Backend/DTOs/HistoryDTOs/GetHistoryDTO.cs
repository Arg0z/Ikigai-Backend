using Ikigai_Backend.DbModels;

namespace Ikigai_Backend.DTOs.HistoryDTOs
{
    public class GetHistoryDTO
    {
        public int UserId { get; set; }
        public int EpisodeID { get; set; }
        public int AnimeId { get; set; }
        public DateTime WatchedAt { get; set; }
    }
}
