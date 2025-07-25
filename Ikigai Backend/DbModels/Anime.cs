namespace Ikigai_Backend.DbModels
{
    public class Anime
    {
        public int Id { get; set; }
        public string AnimeTitle { get; set; } = string.Empty;
        public string Synopsis { get; set; } = string.Empty;
        public DateOnly ReleaseDate { get; set; }
        public string Studio { get; set; } = string.Empty;
        public DateTime LastUpdate { get; set; }
        public List<Episode> Episodes { get; set; } = new();

    }
}
