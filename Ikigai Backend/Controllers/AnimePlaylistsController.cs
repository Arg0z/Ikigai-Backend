using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ikigai_Backend.Database;
using Ikigai_Backend.DbModels;
using Ikigai_Backend.DTOs.AnimePlaylistDTOs;

namespace Ikigai_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnimePlaylistsController : ControllerBase
    {
        private readonly IkigaiDbContext _context;

        public AnimePlaylistsController(IkigaiDbContext context)
        {
            _context = context;
        }

        // GET: api/AnimePlaylists
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetAnimePlaylistDTO>>> GetAnimePlaylists()
        {
            var animePlaylists = await _context.AnimePlaylists
                .Select(ap => new GetAnimePlaylistDTO
                {
                    AnimeId = ap.AnimeId,
                    PlaylistId = ap.PlaylistId
                })
                .ToListAsync();

            return animePlaylists;
        }

        // GET: api/AnimePlaylists/5/10
        [HttpGet("{animeId}/{playlistId}")]
        public async Task<ActionResult<GetAnimePlaylistDTO>> GetAnimePlaylist(int animeId, int playlistId)
        {
            var animePlaylist = await _context.AnimePlaylists
                .FirstOrDefaultAsync(ap => ap.AnimeId == animeId && ap.PlaylistId == playlistId);

            if (animePlaylist == null)
                return NotFound();

            var dto = new GetAnimePlaylistDTO
            {
                AnimeId = animePlaylist.AnimeId,
                PlaylistId = animePlaylist.PlaylistId
            };

            return dto;
        }

        // POST: api/AnimePlaylists
        [HttpPost]
        public async Task<ActionResult<GetAnimePlaylistDTO>> PostAnimePlaylist(PostAnimePlaylistDTO dto)
        {
            var exists = await _context.AnimePlaylists
                .AnyAsync(ap => ap.AnimeId == dto.AnimeId && ap.PlaylistId == dto.PlaylistId);
            if (exists)
                return Conflict("Anime already in playlist.");

            var animePlaylist = new AnimePlaylist
            {
                AnimeId = dto.AnimeId,
                PlaylistId = dto.PlaylistId
            };

            _context.AnimePlaylists.Add(animePlaylist);
            await _context.SaveChangesAsync();

            var getDto = new GetAnimePlaylistDTO
            {
                AnimeId = animePlaylist.AnimeId,
                PlaylistId = animePlaylist.PlaylistId
            };

            return CreatedAtAction(nameof(GetAnimePlaylist), new { animeId = getDto.AnimeId, playlistId = getDto.PlaylistId }, getDto);
        }

        // DELETE: api/AnimePlaylists/5/10
        [HttpDelete("{animeId}/{playlistId}")]
        public async Task<IActionResult> DeleteAnimePlaylist(int animeId, int playlistId)
        {
            var animePlaylist = await _context.AnimePlaylists
                .FirstOrDefaultAsync(ap => ap.AnimeId == animeId && ap.PlaylistId == playlistId);

            if (animePlaylist == null)
                return NotFound();

            _context.AnimePlaylists.Remove(animePlaylist);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
