namespace Ikigai_Backend.DbModels
{
    public class EpisodeVideo
    {
        public int Id { get; set; }
        public string VideoName { get; set; } = string.Empty;
        public string VideoUrl { get; set; } = string.Empty;
        public int EpisodeId { get; set; }
        public Episode Episode { get; set; } = null!;
    }
}
