namespace Ikigai_Backend.DbModels
{
    public class EpisodeSub
    {
        public int Id { get; set; }
        public string SubName { get; set; } = string.Empty;
        public string SubUrl { get; set; } = string.Empty;
        public int EpisodeId { get; set; }
        public Episode Episode { get; set; } = null!;
    }
}
