namespace Ikigai_Backend.DbModels
{
    public class EpisodeAudio
    {
        public int Id { get; set; }
        public string AudioName { get; set; } = string.Empty;
        public string AudioUrl { get; set; }
        public int EpisodeId { get; set; }
        public Episode Episode { get; set; } = null!;
        
    }
}
