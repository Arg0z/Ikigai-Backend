using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ikigai_Backend.Database;
using Ikigai_Backend.DbModels;
using Ikigai_Backend.DTOs.AnimeGenreDTOs;
using Ikigai_Backend.DTOs.AnimeDTOs;

namespace Ikigai_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnimeGenresController : ControllerBase
    {
        private readonly IkigaiDbContext _context;

        public AnimeGenresController(IkigaiDbContext context)
        {
            _context = context;
        }

        // GET: api/AnimeGenres
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetAnimeGenreDTO>>> GetAnimeGenres()
        {
            return await _context.AnimeGenres
                .Include(ag => ag.Anime)
                .Include(ag => ag.Genre)
                .Select(ag => new GetAnimeGenreDTO
                {
                    AnimeId = ag.AnimeId,
                    AnimeTitle = ag.Anime.AnimeTitle,
                    GenreId = ag.GenreId,
                    GenreName = ag.Genre.Name
                })
                .ToListAsync();
        }

        // GET: api/AnimeGenres/1/2
        [HttpGet("{animeId}/{genreId}")]
        public async Task<ActionResult<GetAnimeGenreDTO>> GetAnimeGenre(int animeId, int genreId)
        {
            var ag = await _context.AnimeGenres
                .Include(x => x.Anime)
                .Include(x => x.Genre)
                .Where(x => x.AnimeId == animeId && x.GenreId == genreId)
                .Select(x => new GetAnimeGenreDTO
                {
                    AnimeId = x.AnimeId,
                    AnimeTitle = x.Anime.AnimeTitle,
                    GenreId = x.GenreId,
                    GenreName = x.Genre.Name
                })
                .FirstOrDefaultAsync();

            if (ag == null)
                return NotFound();

            return ag;
        }

        // POST: api/AnimeGenres
        [HttpPost]
        public async Task<ActionResult<GetAnimeGenreDTO>> PostAnimeGenre(PostAnimeGenreDTO dto)
        {
            var animeGenre = new AnimeGenre
            {
                AnimeId = dto.AnimeId,
                GenreId = dto.GenreId
            };

            _context.AnimeGenres.Add(animeGenre);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (AnimeGenreExists(dto.AnimeId, dto.GenreId))
                    return Conflict();
                throw;
            }

            var created = await _context.AnimeGenres
                .Include(ag => ag.Anime)
                .Include(ag => ag.Genre)
                .Where(ag => ag.AnimeId == dto.AnimeId && ag.GenreId == dto.GenreId)
                .Select(ag => new GetAnimeGenreDTO
                {
                    AnimeId = ag.AnimeId,
                    AnimeTitle = ag.Anime.AnimeTitle,
                    GenreId = ag.GenreId,
                    GenreName = ag.Genre.Name
                })
                .FirstOrDefaultAsync();

            return CreatedAtAction(nameof(GetAnimeGenre), new { animeId = dto.AnimeId, genreId = dto.GenreId }, created);
        }

        // DELETE: api/AnimeGenres/1/2
        [HttpDelete("{animeId}/{genreId}")]
        public async Task<IActionResult> DeleteAnimeGenre(int animeId, int genreId)
        {
            var animeGenre = await _context.AnimeGenres.FindAsync(animeId, genreId);
            if (animeGenre == null)
                return NotFound();

            _context.AnimeGenres.Remove(animeGenre);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/AnimeGenres/genre/{genreId}/animes
        [HttpGet("genre/{genreId}/animes")]
        public async Task<ActionResult<IEnumerable<GetAnimeDTO>>> GetAnimesByGenre(int genreId)
        {
            var animes = await _context.AnimeGenres
                .Where(ag => ag.GenreId == genreId)
                .Include(ag => ag.Anime)
                .Select(ag => new GetAnimeDTO
                {
                    Id = ag.Anime.Id,
                    AnimeTitle = ag.Anime.AnimeTitle,
                    Synopsis = ag.Anime.Synopsis,
                    ReleaseDate = ag.Anime.ReleaseDate,
                    Studio = ag.Anime.Studio,
                    LastUpdate = ag.Anime.LastUpdate
                })
                .ToListAsync();

            return animes;
        }

        private bool AnimeGenreExists(int animeId, int genreId)
        {
            return _context.AnimeGenres.Any(e => e.AnimeId == animeId && e.GenreId == genreId);
        }
    }
}