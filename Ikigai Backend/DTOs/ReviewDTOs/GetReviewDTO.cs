namespace Ikigai_Backend.DTOs.ReviewDTOs
{
    public class GetReviewDTO
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int AnimeId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}