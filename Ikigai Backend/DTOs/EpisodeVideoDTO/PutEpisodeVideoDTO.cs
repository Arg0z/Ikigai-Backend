using Ikigai_Backend.Constants;

namespace Ikigai_Backend.DTOs.EpisodeVideoDTO
{
    public class PutEpisodeVideoDTO
    {
        public int Id { get; set; }
        public string VideoName { get; set; } = string.Empty;
        public int EpisodeId { get; set; }
        public VideoResolution Resolution { get; set; }
    }
}
