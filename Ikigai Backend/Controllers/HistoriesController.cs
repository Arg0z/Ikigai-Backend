using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ikigai_Backend.Database;
using Ikigai_Backend.DbModels;
using Ikigai_Backend.DTOs.HistoryDTOs;

namespace Ikigai_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HistoriesController : ControllerBase
    {
        private readonly IkigaiDbContext _context;

        public HistoriesController(IkigaiDbContext context)
        {
            _context = context;
        }

        // GET: api/Histories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetHistoryDTO>>> GetHistories()
        {
            return await _context.Histories
                .Include(h => h.Episode)
                .Select(h => new GetHistoryDTO
                {
                    UserId = h.UserId,
                    EpisodeID = h.EpisodeID,
                    WatchedAt = h.WatchedAt,
                    AnimeId = h.Episode.AnimeId
                })
                .ToListAsync();
        }

        // GET: api/Histories/5
        [HttpGet("{idUser}/{idEpisode}")]
        public async Task<ActionResult<GetHistoryDTO>> GetHistory(int idUser, int idEpisode)
        {
            var history = await _context.Histories
                .Where(h => h.UserId == idUser && h.EpisodeID == idEpisode)
                .Select(h => new GetHistoryDTO
                {
                    UserId = h.UserId,
                    EpisodeID = h.EpisodeID,
                    WatchedAt = h.WatchedAt,
                    AnimeId = h.Episode.AnimeId
                })
                .FirstOrDefaultAsync();

            if (history == null)
            {
                return NotFound();
            }

            return history;
        }

        // PUT: api/Histories/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{idUser}/{idEpisode}")]
        public async Task<IActionResult> PutHistory(int userId, int episodeId, [FromBody] UpdateHistoryDTO history)
        {
            if (userId != history.UserId || episodeId != history.EpisodeID)
            {
                return BadRequest("User ID and Episode ID in the URL must match the data provided.");
            }

            // Find the existing entity
            var existingHistory = await _context.Histories
                .FirstOrDefaultAsync(h => h.UserId == userId && h.EpisodeID == episodeId);

            if (existingHistory == null)
            {
                return NotFound();
            }

            // Update properties
            existingHistory.WatchedAt = DateTime.UtcNow;

            // Save changes
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Histories
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<History>> PostHistory([FromBody] PostHistoryDTO dto)
        {
            // Validate existence of related entities if needed
            var history = new History
            {
                UserId = dto.UserId,
                EpisodeID = dto.EpisodeID,
                // Set other properties as needed
            };

            _context.Histories.Add(history);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (HistoryExists(history.UserId, history.EpisodeID))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetHistory", new { idUser = history.UserId, idEpisode = history.EpisodeID }, history);
        }

        // DELETE: api/Histories/{userId}/{episodeId}
        [HttpDelete("{userId}/{episodeId}")]
        public async Task<IActionResult> DeleteHistory(int userId, int episodeId)
        {
            var history = await _context.Histories
                .FirstOrDefaultAsync(h => h.UserId == userId && h.EpisodeID == episodeId);
            if (history == null)
            {
                return NotFound();
            }

            _context.Histories.Remove(history);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool HistoryExists(int userId, int episodeId)
        {
            return _context.Histories.Any(e => e.UserId == userId && e.EpisodeID == episodeId);
        }

        // GET: api/Histories/user/{userId}
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<GetHistoryDTO>>> GetUserHistories(int userId)
        {
            var histories = await _context.Histories
                .Where(h => h.UserId == userId)
                .Include(h => h.Episode)
                .OrderByDescending(h => h.WatchedAt)
                .Select(h => new GetHistoryDTO
                {
                    UserId = h.UserId,
                    EpisodeID = h.EpisodeID,
                    WatchedAt = h.WatchedAt,
                    AnimeId = h.Episode.AnimeId
                })
                .ToListAsync();

            if (!histories.Any())
            {
                return NotFound();
            }
            return histories;
        }

        // GET: api/Histories/animes-most-watched
        [HttpGet("animes-most-watched")]
        public async Task<ActionResult<IEnumerable<MostWatchedAnimeDTO>>> GetMostWatchedAnimes([FromQuery] string borderDate)
        {
            if (!DateOnly.TryParse(borderDate, out var date))
                return BadRequest("Invalid date format. Use YYYY-MM-DD.");

            var mostWatchedAnimes = await _context.Histories
                .Where(h => h.WatchedAt >= date.ToDateTime(TimeOnly.MinValue))
                .GroupBy(h => h.Episode.AnimeId)
                .Select(g => new MostWatchedAnimeDTO
                {
                    AnimeId = g.Key,
                    WatchCount = g.Count()
                })
                .OrderByDescending(m => m.WatchCount)
                .Take(10)
                .ToListAsync();

            return mostWatchedAnimes;
        }
    }
}
