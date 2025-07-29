namespace Ikigai_Backend.DbModels
{
    public class Playlist
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<AnimePlaylist> AnimePlaylists { get; set; } = new List<AnimePlaylist>();
    }
}
