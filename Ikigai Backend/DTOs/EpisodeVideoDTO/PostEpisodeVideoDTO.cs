using Ikigai_Backend.Constants;
using Microsoft.AspNetCore.Http;

namespace Ikigai_Backend.DTOs.EpisodeVideoDTO
{
    public class PostEpisodeVideoDTO
    {
        public string VideoName { get; set; }
        public int EpisodeId { get; set; }
        public VideoResolution Resolution { get; set; }
        public IFormFile VideoFile { get; set; }
    }
}
