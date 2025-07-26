using Ikigai_Backend.Database;
using Ikigai_Backend.DbModels;
using Ikigai_Backend.DTOs.EpisodeSubDTOs;
using Ikigai_Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Ikigai_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EpisodeSubsController : ControllerBase
    {
        private readonly IkigaiDbContext _context;
        private readonly AnimeService _animeService;

        public EpisodeSubsController(IkigaiDbContext context, AnimeService animeService)
        {
            _context = context;
            _animeService = animeService;
        }

        // GET: api/EpisodeSubs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetEpisodeSubDTO>>> GetEpisodeSub()
        {
            var episodeSubs = await _context.EpisodeSub
                .Select(es => new GetEpisodeSubDTO
                {
                    Id = es.Id,
                    SubName = es.SubName,
                    EpisodeId = es.EpisodeId
                })
                .ToListAsync();

            return episodeSubs;
        }

        // GET: api/EpisodeSubs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GetEpisodeSubDTO>> GetEpisodeSub(int id)
        {
            var episodeSub = await _context.EpisodeSub.FindAsync(id);

            if (episodeSub == null)
            {
                return NotFound();
            }

            var episodeSubDTO = new GetEpisodeSubDTO
            {
                Id = episodeSub.Id,
                SubName = episodeSub.SubName,
                EpisodeId = episodeSub.EpisodeId
            };

            return episodeSubDTO;
        }

        // GET: api/EpisodeSubs/byEpisode/5
        [HttpGet("byEpisode/{episodeId}")]
        public async Task<ActionResult<IEnumerable<GetEpisodeSubDTO>>> GetSubsByEpisode(int episodeId)
        {
            var subs = await _context.EpisodeSub
                .Where(es => es.EpisodeId == episodeId)
                .Select(es => new GetEpisodeSubDTO
                {
                    Id = es.Id,
                    SubName = es.SubName,
                    EpisodeId = es.EpisodeId
                })
                .ToListAsync();

            if (!subs.Any())
                return NotFound();

            return subs;
        }

        // PUT: api/EpisodeSubs/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutEpisodeSub(int id, PutEpisodeSubDTO dto)
        {
            if (id != dto.Id)
            {
                return BadRequest();
            }

            var existingEpisodeSub = await _context.EpisodeSub.FindAsync(id);
            if (existingEpisodeSub == null)
            {
                return NotFound();
            }

            existingEpisodeSub.SubName = dto.SubName;
            existingEpisodeSub.EpisodeId = dto.EpisodeId;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EpisodeSubExists(id))
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

        // POST: api/EpisodeSubs
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<GetEpisodeSubDTO>> PostEpisodeSub(
            [FromForm] PostEpisodeSubDTO episodeSubDTO,
            [FromForm] IFormFile subFile)
        {
            if (subFile == null || subFile.Length == 0)
                return BadRequest("No subtitle file uploaded.");

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "UploadedSubs");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(subFile.FileName)}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await subFile.CopyToAsync(stream);
            }

            var episodeSub = new EpisodeSub
            {
                SubName = episodeSubDTO.SubName,
                EpisodeId = episodeSubDTO.EpisodeId,
                SubUrl = $"/UploadedSubs/{uniqueFileName}"
            };

            _context.EpisodeSub.Add(episodeSub);
            await _context.SaveChangesAsync();

            var episode = await _context.Episodes.FindAsync(episodeSub.EpisodeId);
            if (episode == null)
                return NotFound("Episode not found.");

            int animeId = episode.AnimeId;

            await _animeService.UpdateAnimeLastUpdateAsync(animeId)

            var getEpisodeSubDTO = new GetEpisodeSubDTO
            {
                Id = episodeSub.Id,
                SubName = episodeSub.SubName,
                EpisodeId = episodeSub.EpisodeId
            };

            return CreatedAtAction("GetEpisodeSub", new { id = getEpisodeSubDTO.Id }, getEpisodeSubDTO);
        }

        // DELETE: api/EpisodeSubs/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteEpisodeSub(int id)
        {
            var episodeSub = await _context.EpisodeSub.FindAsync(id);
            if (episodeSub == null)
            {
                return NotFound();
            }

            _context.EpisodeSub.Remove(episodeSub);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/EpisodeSubs/stream/5
        [HttpGet("stream/{id}")]
        public async Task<IActionResult> StreamSub(int id)
        {
            var episodeSub = await _context.EpisodeSub.FindAsync(id);
            if (episodeSub == null || string.IsNullOrEmpty(episodeSub.SubUrl))
                return NotFound();

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), episodeSub.SubUrl.TrimStart('/'));
            if (!System.IO.File.Exists(filePath))
                return NotFound();

            var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            var mimeType = "text/vtt"; // Adjust if you support other subtitle formats

            return File(stream, mimeType, enableRangeProcessing: true);
        }

        private bool EpisodeSubExists(int id)
        {
            return _context.EpisodeSub.Any(e => e.Id == id);
        }
    }
}
