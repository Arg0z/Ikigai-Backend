using Ikigai_Backend.DbModels;

namespace Ikigai_Backend.DTOs.EpisodeDTOs
{
    public class GetEpisodeDTO
    {
        public int Id { get; set; }
        public int EpisodeNumber { get; set; }
        public string Title { get; set; } = string.Empty;
        public int AnimeId { get; set; }
        public Boolean isMovie { get; set; } = false;
    }
}
