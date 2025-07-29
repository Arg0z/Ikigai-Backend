namespace Ikigai_Backend.DbModels
{
    public class Genre
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<AnimeGenre> AnimeGenres { get; set; } = new(); // Add this line
    }
}
