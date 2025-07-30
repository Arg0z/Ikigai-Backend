using System.Collections.Generic;

namespace Ikigai_Backend.DTOs.PlaylistDTOs
{
    public class PutPlaylistDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<int> AnimeIds { get; set; } = new();
    }
}