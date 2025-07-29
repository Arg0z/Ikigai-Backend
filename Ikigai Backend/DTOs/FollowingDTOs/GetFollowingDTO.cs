namespace Ikigai_Backend.DTOs.FollowingDTOs
{
    public class GetFollowingDTO
    {
        public int UserId { get; set; }
        public int AnimeId { get; set; }
        public string AnimeTitle { get; set; }
    }
}