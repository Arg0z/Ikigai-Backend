namespace Ikigai_Backend.DTOs.EpisodeVideoDTO
{
    public class PostEpisodeVideoDTO
    {
        public string VideoName { get; set; }
        public int EpisodeId { get; set; }
        public IFormFile VideoFile { get; set; }
    }
}
