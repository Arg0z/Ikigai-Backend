namespace Ikigai_Backend.DTOs.EpisodeDTOs
{
    public class PostEpisodeDTO
    {
        public int EpisodeNumber { get; set; }
        public string Title { get; set; } = string.Empty;
        public int AnimeId { get; set; }
        public Boolean isMovie { get; set; } = false;
    }
}
