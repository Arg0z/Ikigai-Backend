namespace Ikigai_Backend.DTOs.EpisodeSubDTOs
{
    public class PutEpisodeSubDTO
    {
        public int Id { get; set; }
        public string SubName { get; set; }
        public int EpisodeId { get; set; }
    }
}