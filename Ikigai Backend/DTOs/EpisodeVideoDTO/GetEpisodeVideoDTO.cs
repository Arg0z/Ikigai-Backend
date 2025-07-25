using Ikigai_Backend.DbModels;

namespace Ikigai_Backend.DTOs.EpisodeVideoDTO
{
    public class GetEpisodeVideoDTO
    {
        public int Id { get; set; }
        public string VideoName { get; set; } = string.Empty;
        public int EpisodeId { get; set; }
    }
}
