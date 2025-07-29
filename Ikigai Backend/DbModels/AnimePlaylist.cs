namespace Ikigai_Backend.DbModels
{
    public class AnimePlaylist
    {
        public int AnimeId { get; set; }
        public Anime Anime { get; set; }
        public int PlaylistId { get; set; }
        public Playlist Playlist { get; set; }
    }
}
