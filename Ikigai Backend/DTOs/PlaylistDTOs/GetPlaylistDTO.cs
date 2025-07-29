using System;
using System.Collections.Generic;

namespace Ikigai_Backend.DTOs.PlaylistDTOs
{
    public class GetPlaylistDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public List<int> AnimeIds { get; set; } = new();
    }
}