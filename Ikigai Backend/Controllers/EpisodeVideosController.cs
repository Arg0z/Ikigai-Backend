using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ikigai_Backend.Database;
using Ikigai_Backend.DbModels;
using Ikigai_Backend.DTOs.EpisodeVideoDTO;
using Ikigai_Backend.Services;

namespace Ikigai_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EpisodeVideosController : ControllerBase
    {
        private readonly IkigaiDbContext _context;
        private readonly AnimeService _animeService;

        public EpisodeVideosController(IkigaiDbContext context, AnimeService animeService)
        {
            _context = context;
            _animeService = animeService;
        }

        // GET: api/EpisodeVideos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetEpisodeVideoDTO>>> GetEpisodeVideo()
        {
            var episodeVideos = await _context.EpisodeVideo.Select(ev => new GetEpisodeVideoDTO
            {
                Id = ev.Id,
                VideoName = ev.VideoName,
                EpisodeId = ev.EpisodeId
            })
            .ToListAsync();

            return episodeVideos;
        }

        // GET: api/EpisodeVideos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GetEpisodeVideoDTO>> GetEpisodeVideo(int id)
        {
            var episodeVideo = await _context.EpisodeVideo.FindAsync(id);

            if (episodeVideo == null)
            {
                return NotFound();
            }

            var episodeVideoDTO = new GetEpisodeVideoDTO
            {
                Id = episodeVideo.Id,
                VideoName = episodeVideo.VideoName,
                EpisodeId = episodeVideo.EpisodeId
            };

            return episodeVideoDTO;
        }

        // PUT: api/EpisodeVideos/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEpisodeVideo(int id, PutEpisodeVideoDTO episodeVideoDTO)
        {
            if (id != episodeVideoDTO.Id)
            {
                return BadRequest();
            }

            var existingEpisodeVideo = await _context.EpisodeVideo.FindAsync(id);
            if (existingEpisodeVideo == null)
            {
                return NotFound();
            }

            existingEpisodeVideo.VideoName = episodeVideoDTO.VideoName;
            existingEpisodeVideo.EpisodeId = episodeVideoDTO.EpisodeId;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EpisodeVideoExists(id))
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

        // POST: api/EpisodeVideos
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<GetEpisodeVideoDTO>> PostEpisodeVideo(
            [FromForm] PostEpisodeVideoDTO episodeVideoDTO,
            [FromForm] IFormFile videoFile)
        {
            if (videoFile == null || videoFile.Length == 0)
                return BadRequest("No video file uploaded.");

            // Save the file to a folder in your project (e.g., "UploadedVideos")
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "UploadedVideos");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(videoFile.FileName)}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await videoFile.CopyToAsync(stream);
            }

            // Save the video info in the database
            var episodeVideo = new EpisodeVideo
            {
                VideoName = episodeVideoDTO.VideoName,
                EpisodeId = episodeVideoDTO.EpisodeId,
                VideoUrl = $"/UploadedVideos/{uniqueFileName}" // This is the relative URL for serving the file
            };

            _context.EpisodeVideo.Add(episodeVideo);
            await _context.SaveChangesAsync();

            await _animeService.UpdateAnimeLastUpdateAsync(episodeVideoDTO.EpisodeId);

            var getEpisodeVideoDTO = new GetEpisodeVideoDTO
            {
                Id = episodeVideo.Id,
                VideoName = episodeVideo.VideoName,
                EpisodeId = episodeVideo.EpisodeId
            };

            return CreatedAtAction("GetEpisodeVideo", new { id = getEpisodeVideoDTO.Id }, getEpisodeVideoDTO);
        }

        // DELETE: api/EpisodeVideos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEpisodeVideo(int id)
        {
            var episodeVideo = await _context.EpisodeVideo.FindAsync(id);
            if (episodeVideo == null)
            {
                return NotFound();
            }

            _context.EpisodeVideo.Remove(episodeVideo);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EpisodeVideoExists(int id)
        {
            return _context.EpisodeVideo.Any(e => e.Id == id);
        }
    }
}
