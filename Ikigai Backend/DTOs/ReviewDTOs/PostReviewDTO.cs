namespace Ikigai_Backend.DTOs.ReviewDTOs
{
    public class PostReviewDTO
    {
        public int UserId { get; set; }
        public int AnimeId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
    }
}