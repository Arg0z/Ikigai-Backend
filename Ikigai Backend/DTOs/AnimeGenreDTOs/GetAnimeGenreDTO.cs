namespace Ikigai_Backend.DTOs.AnimeGenreDTOs
{
    public class GetAnimeGenreDTO
    {
        public int AnimeId { get; set; }
        public string AnimeTitle { get; set; }
        public int GenreId { get; set; }
        public string GenreName { get; set; }
    }
}