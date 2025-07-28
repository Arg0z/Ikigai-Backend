using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ikigai_Backend.Database;
using Ikigai_Backend.DbModels;
using Ikigai_Backend.DTOs.EpisodeDTOs;
using Microsoft.AspNetCore.Authorization;
using Ikigai_Backend.Services;

namespace Ikigai_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EpisodesController : ControllerBase
    {
        private readonly IkigaiDbContext _context;
        private readonly AnimeService _animeService;

        public EpisodesController(IkigaiDbContext context, AnimeService animeService)
        {
            _context = context;
            _animeService = animeService;

        }

        // GET: api/Episodes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetEpisodeDTO>>> GetEpisodes()
        {
            var episodes = await _context.Episodes
                .Select(e => new GetEpisodeDTO
                {
                    Id = e.Id,
                    SeasonNumber = e.SeasonNumber,
                    EpisodeNumber = e.EpisodeNumber,
                    Title = e.Title,
                    AnimeId = e.AnimeId,
                    isMovie = e.isMovie
                })
                .ToListAsync();
            return episodes;
        }

        // GET: api/Episodes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GetEpisodeDTO>> GetEpisode(int id)
        {
            var episode = await _context.Episodes.FindAsync(id);

            if (episode == null)
            {
                return NotFound();
            }

            var episodeDTO = new GetEpisodeDTO
            {
                Id = episode.Id,
                SeasonNumber = episode.SeasonNumber,
                EpisodeNumber = episode.EpisodeNumber,
                Title = episode.Title,
                AnimeId = episode.AnimeId,
                isMovie = episode.isMovie
            };

            return episodeDTO;
        }

        // PUT: api/Episodes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutEpisode(int id, PutEpisodeDTO dto)
        {
            if (id != dto.Id)
            {
                return BadRequest();
            }

            var episode = await _context.Episodes.FindAsync(id);
            if (episode == null)
            {
                return NotFound();
            }

            episode.SeasonNumber = dto.SeasonNumber;
            episode.EpisodeNumber = dto.EpisodeNumber;
            episode.Title = dto.Title;
            episode.AnimeId = dto.AnimeId;
            episode.isMovie = dto.isMovie;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EpisodeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Episodes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<GetEpisodeDTO>> PostEpisode(PostEpisodeDTO episodePostDTO)
        {
            var episode = new Episode
            {
                SeasonNumber = episodePostDTO.SeasonNumber,
                EpisodeNumber = episodePostDTO.EpisodeNumber,
                Title = episodePostDTO.Title,
                AnimeId = episodePostDTO.AnimeId,
                isMovie = episodePostDTO.isMovie
            };
            _context.Episodes.Add(episode);
            await _context.SaveChangesAsync();

            await _animeService.UpdateAnimeLastUpdateAsync(episodePostDTO.AnimeId);

            var getEpisodeDTO = new GetEpisodeDTO
            {
                Id = episode.Id,
                SeasonNumber = episode.SeasonNumber,
                EpisodeNumber = episode.EpisodeNumber,
                Title = episode.Title,
                AnimeId = episode.AnimeId,
                isMovie = episode.isMovie
            };

            return CreatedAtAction("GetEpisode", new { id = getEpisodeDTO.Id }, getEpisodeDTO);
        }

        // DELETE: api/Episodes/5
        [HttpDelete("{id}")]
        [Authorize (Roles = "Admin")]
        public async Task<IActionResult> DeleteEpisode(int id)
        {
            var episode = await _context.Episodes.FindAsync(id);
            if (episode == null)
            {
                return NotFound();
            }

            _context.Episodes.Remove(episode);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EpisodeExists(int id)
        {
            return _context.Episodes.Any(e => e.Id == id);
        }

        // GET: api/Episodes/episodeByAnime/{animeId}
        [HttpGet("episodeByAnime/{animeId}")]
        public async Task<ActionResult<IEnumerable<GetEpisodeDTO>>> GetEpisodesByAnime(int animeId)
        {
            var episodes = await _context.Episodes
                .Where(e => e.AnimeId == animeId)
                .Select(e => new GetEpisodeDTO
                {
                    Id = e.Id,
                    SeasonNumber = e.SeasonNumber,
                    EpisodeNumber = e.EpisodeNumber,
                    Title = e.Title,
                    AnimeId = e.AnimeId,
                    isMovie = e.isMovie
                })
                .ToListAsync();
            if (episodes == null || !episodes.Any())
            {
                return NotFound();
            }
            return episodes;
        }
    }
}
