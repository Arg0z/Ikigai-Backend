namespace Ikigai_Backend.DTOs.EpisodeAudioDTOs
{
    public class PutEpisodeAudioDTO
    {
        public int Id { get; set; }
        public string AudioName { get; set; }
        public int EpisodeId { get; set; }
    }
}
