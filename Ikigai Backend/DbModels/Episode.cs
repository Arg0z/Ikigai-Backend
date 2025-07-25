namespace Ikigai_Backend.DbModels
{
    public class Episode
    {
        public int Id { get; set; }
        public int EpisodeNumber { get; set; }
        public string Title { get; set; } = string.Empty;
        public int AnimeId { get; set; }
        public Anime Anime { get; set; } = null!;
        public Boolean isMovie { get; set; } = false;

        public List<EpisodeVideo> EpisodeVideos { get; set; } = new();
        public List<EpisodeAudio> EpisodeAudios { get; set; } = new();
        public List<EpisodeSub> EpisodeSubtitles { get; set; } = new();
    }
}
