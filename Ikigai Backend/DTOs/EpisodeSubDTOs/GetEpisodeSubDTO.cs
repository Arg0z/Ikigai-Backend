namespace Ikigai_Backend.DTOs.EpisodeSubDTOs
{
    public class GetEpisodeSubDTO
    {
        public int Id { get; set; }
        public string SubName { get; set; }
        public int EpisodeId { get; set; }
        // Optionally add SubUrl if you want to expose it
    }
}