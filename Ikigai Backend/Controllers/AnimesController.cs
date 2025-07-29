using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ikigai_Backend.Database;
using Ikigai_Backend.DbModels;
using Ikigai_Backend.DTOs.AnimeDTOs;
using Microsoft.AspNetCore.Authorization;

namespace Ikigai_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnimesController : ControllerBase
    {
        private readonly IkigaiDbContext _context;

        public AnimesController(IkigaiDbContext context)
        {
            _context = context;
        }

        // GET: api/Animes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetAnimeDTO>>> GetAnimes()
        {
            var animes = await _context.Animes
                .Select(a => new GetAnimeDTO
                {
                    Id = a.Id,
                    AnimeTitle = a.AnimeTitle,
                    Synopsis = a.Synopsis,
                    ReleaseDate = a.ReleaseDate,
                    UploadDate = a.UploadDate,
                    IsOngoing = a.IsOngoing,
                    Studio = a.Studio,
                    LastUpdate = a.LastUpdate,
                    ImageUrl = a.ImageUrl
                })
                .ToListAsync();

            return animes;
        }

        // GET: api/Animes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GetAnimeDTO>> GetAnime(int id)
        {
            var animeDB = await _context.Animes.FindAsync(id);

            if (animeDB == null)
            {
                return NotFound();
            }

            var anime = new GetAnimeDTO
            {
                Id = animeDB.Id,
                AnimeTitle = animeDB.AnimeTitle,
                Synopsis = animeDB.Synopsis,
                ReleaseDate = animeDB.ReleaseDate,
                UploadDate = animeDB.UploadDate,
                IsOngoing = animeDB.IsOngoing,
                Studio = animeDB.Studio,
                LastUpdate = animeDB.LastUpdate,
                ImageUrl = animeDB.ImageUrl
            };

            return anime;
        }

        // GET: api/Animes/searchByName
        [HttpGet("searchByName")]
        public async Task<ActionResult<IEnumerable<GetAnimeDTO>>> SearchAnimes([FromQuery] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest("Search term is required.");

            var animes = await _context.Animes
                .Where(a => a.AnimeTitle.Contains(name))
                .Select(a => new GetAnimeDTO
                {
                    Id = a.Id,
                    AnimeTitle = a.AnimeTitle,
                    Synopsis = a.Synopsis,
                    ReleaseDate = a.ReleaseDate,
                    UploadDate = a.UploadDate,
                    IsOngoing = a.IsOngoing,
                    Studio = a.Studio,
                    LastUpdate = a.LastUpdate,
                    ImageUrl = a.ImageUrl
                })
                .ToListAsync();

            if (!animes.Any())
                return NotFound();

            return animes;
        }

        // PUT: api/Animes/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutAnime(int id, [FromForm] UpdateAnimeDTO dto)
        {
            if (id != dto.Id)
            {
                return BadRequest();
            }

            var anime = await _context.Animes.FindAsync(id);
            if (anime == null)
            {
                return NotFound();
            }

            anime.AnimeTitle = dto.AnimeTitle;
            anime.Synopsis = dto.Synopsis;
            anime.ReleaseDate = dto.ReleaseDate;
            anime.UploadDate = dto.UploadDate;
            anime.IsOngoing = dto.IsOngoing;
            anime.Studio = dto.Studio;

            if (dto.ImageFile != null && dto.ImageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "UploadedAnimeImages");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(dto.ImageFile.FileName)}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.ImageFile.CopyToAsync(stream);
                }
                anime.ImageUrl = $"/UploadedAnimeImages/{uniqueFileName}";
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AnimeExists(id))
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

        // POST: api/Animes
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<GetAnimeDTO>> PostAnime([FromForm] PostAnimeDTO animePostDTO)
        {
            if (animePostDTO == null)
            {
                return BadRequest("Anime data is null.");
            }

            string imageUrl = string.Empty;
            if (animePostDTO.ImageFile != null && animePostDTO.ImageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "UploadedAnimeImages");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(animePostDTO.ImageFile.FileName)}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await animePostDTO.ImageFile.CopyToAsync(stream);
                }
                imageUrl = $"/UploadedAnimeImages/{uniqueFileName}";
            }

            var anime = new Anime
            {
                AnimeTitle = animePostDTO.AnimeTitle,
                Synopsis = animePostDTO.Synopsis,
                ReleaseDate = animePostDTO.ReleaseDate,
                UploadDate = animePostDTO.UploadDate,
                IsOngoing = animePostDTO.IsOngoing,
                Studio = animePostDTO.Studio,
                LastUpdate = animePostDTO.LastUpdate,
                ImageUrl = imageUrl
            };

            _context.Animes.Add(anime);
            await _context.SaveChangesAsync();

            var animeGetDto = new GetAnimeDTO
            {
                Id = anime.Id,
                AnimeTitle = anime.AnimeTitle,
                Synopsis = anime.Synopsis,
                ReleaseDate = anime.ReleaseDate,
                UploadDate = anime.UploadDate,
                IsOngoing = anime.IsOngoing,
                Studio = anime.Studio,
                LastUpdate = anime.LastUpdate,
                ImageUrl = anime.ImageUrl
            };

            return CreatedAtAction("GetAnime", new { id = animeGetDto.Id }, animeGetDto);
        }

        // DELETE: api/Animes/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAnime(int id)
        {
            var anime = await _context.Animes.FindAsync(id);
            if (anime == null)
            {
                return NotFound();
            }

            _context.Animes.Remove(anime);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/Animes/image/5
        [HttpGet("image/{id}")]
        public async Task<IActionResult> GetAnimeImage(int id)
        {
            var anime = await _context.Animes.FindAsync(id);
            if (anime == null || string.IsNullOrEmpty(anime.ImageUrl))
                return NotFound();

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), anime.ImageUrl.TrimStart('/'));
            if (!System.IO.File.Exists(filePath))
                return NotFound();

            var mimeType = "image/jpeg"; // Adjust if you support other formats
            var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            return File(stream, mimeType, enableRangeProcessing: false);
        }

        private bool AnimeExists(int id)
        {
            return _context.Animes.Any(e => e.Id == id);
        }
    }
}
