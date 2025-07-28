using Ikigai_Backend.Database;
using Ikigai_Backend.DbModels;
using Ikigai_Backend.DTOs.EpisodeAudioDTOs;
using Ikigai_Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ikigai_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EpisodeAudiosController : ControllerBase
    {
        private readonly IkigaiDbContext _context;
        private readonly AnimeService _animeService;

        public EpisodeAudiosController(IkigaiDbContext context, AnimeService animeService)
        {
            _context = context;
            _animeService = animeService;
        }

        // GET: api/EpisodeAudios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetEpisodeAudioDTO>>> GetEpisodeAudio()
        {
            var episodeAudios = await _context.EpisodeAudio
                .Select(ea => new GetEpisodeAudioDTO
                {
                    Id = ea.Id,
                    AudioName = ea.AudioName,
                    EpisodeId = ea.EpisodeId
                    // Optionally add AudioUrl if you want to expose it
                })
                .ToListAsync();

            return episodeAudios;
        }

        // GET: api/EpisodeAudios/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GetEpisodeAudioDTO>> GetEpisodeAudio(int id)
        {
            var episodeAudio = await _context.EpisodeAudio.FindAsync(id);

            if (episodeAudio == null)
            {
                return NotFound();
            }

            var episodeAudioDTO = new GetEpisodeAudioDTO
            {
                Id = episodeAudio.Id,
                AudioName = episodeAudio.AudioName,
                EpisodeId = episodeAudio.EpisodeId
                // Optionally add AudioUrl if you want to expose it
            };

            return episodeAudioDTO;
        }

        // GET: api/EpisodeAudios/stream/5
        [HttpGet("stream/{id}")]
        public async Task<IActionResult> StreamAudio(int id)
        {
            var episodeAudio = await _context.EpisodeAudio.FindAsync(id);
            if (episodeAudio == null || string.IsNullOrEmpty(episodeAudio.AudioUrl))
                return NotFound();

            // Get the absolute path to the audio file
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), episodeAudio.AudioUrl.TrimStart('/'));
            if (!System.IO.File.Exists(filePath))
                return NotFound();

            var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            var mimeType = "audio/mpeg"; // Adjust if you support other formats

            return File(stream, mimeType, enableRangeProcessing: true);
        }

        // PUT: api/EpisodeAudios/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutEpisodeAudio(int id, PutEpisodeAudioDTO dto)
        {
            if (id != dto.Id)
            {
                return BadRequest();
            }

            var existingEpisodeAudio = await _context.EpisodeAudio.FindAsync(id);
            if (existingEpisodeAudio == null)
            {
                return NotFound();
            }

            existingEpisodeAudio.AudioName = dto.AudioName;
            existingEpisodeAudio.EpisodeId = dto.EpisodeId;
            // Optionally update AudioUrl if you allow changing the file

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EpisodeAudioExists(id))
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

        // POST: api/EpisodeAudios
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<GetEpisodeAudioDTO>> PostEpisodeAudio([FromForm] PostEpisodeAudioDTO dto)
        {
            if (dto.AudioFile == null || dto.AudioFile.Length == 0)
                return BadRequest("No audio file uploaded.");

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "UploadedAudios");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(dto.AudioFile.FileName)}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await dto.AudioFile.CopyToAsync(stream);
            }

            var episodeAudio = new EpisodeAudio
            {
                AudioName = dto.AudioName,
                EpisodeId = dto.EpisodeId,
                AudioUrl = $"/UploadedAudios/{uniqueFileName}"
            };

            _context.EpisodeAudio.Add(episodeAudio);
            await _context.SaveChangesAsync();

            var episode = await _context.Episodes.FindAsync(episodeAudio.EpisodeId);
            if (episode == null)
                return NotFound("Episode not found.");

            int animeId = episode.AnimeId;

            await _animeService.UpdateAnimeLastUpdateAsync(animeId);

            var getEpisodeAudioDTO = new GetEpisodeAudioDTO
            {
                Id = episodeAudio.Id,
                AudioName = episodeAudio.AudioName,
                EpisodeId = episodeAudio.EpisodeId
                // Optionally add AudioUrl if you want to expose it
            };

            return CreatedAtAction("GetEpisodeAudio", new { id = getEpisodeAudioDTO.Id }, getEpisodeAudioDTO);
        }

        // DELETE: api/EpisodeAudios/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteEpisodeAudio(int id)
        {
            var episodeAudio = await _context.EpisodeAudio.FindAsync(id);
            if (episodeAudio == null)
            {
                return NotFound();
            }

            _context.EpisodeAudio.Remove(episodeAudio);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/EpisodeAudios/byEpisode/5
        [HttpGet("byEpisode/{episodeId}")]
        public async Task<ActionResult<IEnumerable<GetEpisodeAudioDTO>>> GetAudiosByEpisode(int episodeId)
        {
            var audios = await _context.EpisodeAudio
                .Where(ea => ea.EpisodeId == episodeId)
                .Select(ea => new GetEpisodeAudioDTO
                {
                    Id = ea.Id,
                    AudioName = ea.AudioName,
                    EpisodeId = ea.EpisodeId
                })
                .ToListAsync();

            if (!audios.Any())
                return NotFound();

            return audios;
        }

        private bool EpisodeAudioExists(int id)
        {
            return _context.EpisodeAudio.Any(e => e.Id == id);
        }
    }
}
