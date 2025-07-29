using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ikigai_Backend.Database;
using Ikigai_Backend.DbModels;
using Ikigai_Backend.DTOs.PlaylistDTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ikigai_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlaylistsController : ControllerBase
    {
        private readonly IkigaiDbContext _context;

        public PlaylistsController(IkigaiDbContext context)
        {
            _context = context;
        }

        // GET: api/Playlists
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetPlaylistDTO>>> GetPlaylists()
        {
            var playlists = await _context.Playlists
                .Include(p => p.AnimePlaylists)
                .Select(p => new GetPlaylistDTO
                {
                    Id = p.Id,
                    UserId = p.UserId,
                    Name = p.Name,
                    Description = p.Description,
                    CreatedAt = p.CreatedAt,
                    AnimeIds = p.AnimePlaylists.Select(ap => ap.AnimeId).ToList()
                })
                .ToListAsync();

            return playlists;
        }

        // GET: api/Playlists/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GetPlaylistDTO>> GetPlaylist(int id)
        {
            var playlist = await _context.Playlists
                .Include(p => p.AnimePlaylists)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (playlist == null)
                return NotFound();

            var dto = new GetPlaylistDTO
            {
                Id = playlist.Id,
                UserId = playlist.UserId,
                Name = playlist.Name,
                Description = playlist.Description,
                CreatedAt = playlist.CreatedAt,
                AnimeIds = playlist.AnimePlaylists.Select(ap => ap.AnimeId).ToList()
            };

            return dto;
        }

        // POST: api/Playlists
        [HttpPost]
        public async Task<ActionResult<GetPlaylistDTO>> PostPlaylist(PostPlaylistDTO dto)
        {
            var playlist = new Playlist
            {
                UserId = dto.UserId,
                Name = dto.Name,
                Description = dto.Description,
                CreatedAt = DateTime.UtcNow,
                AnimePlaylists = dto.AnimeIds.Select(animeId => new AnimePlaylist { AnimeId = animeId }).ToList()
            };

            _context.Playlists.Add(playlist);
            await _context.SaveChangesAsync();

            var getDto = new GetPlaylistDTO
            {
                Id = playlist.Id,
                UserId = playlist.UserId,
                Name = playlist.Name,
                Description = playlist.Description,
                CreatedAt = playlist.CreatedAt,
                AnimeIds = playlist.AnimePlaylists.Select(ap => ap.AnimeId).ToList()
            };

            return CreatedAtAction(nameof(GetPlaylist), new { id = getDto.Id }, getDto);
        }

        // PUT: api/Playlists/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPlaylist(int id, PutPlaylistDTO dto)
        {
            if (id != dto.Id)
                return BadRequest();

            var playlist = await _context.Playlists
                .Include(p => p.AnimePlaylists)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (playlist == null)
                return NotFound();

            playlist.Name = dto.Name;
            playlist.Description = dto.Description;

            // Update AnimePlaylists
            playlist.AnimePlaylists.Clear();
            foreach (var animeId in dto.AnimeIds)
            {
                playlist.AnimePlaylists.Add(new AnimePlaylist { AnimeId = animeId, PlaylistId = id });
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/Playlists/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlaylist(int id)
        {
            var playlist = await _context.Playlists.FindAsync(id);
            if (playlist == null)
                return NotFound();

            _context.Playlists.Remove(playlist);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}