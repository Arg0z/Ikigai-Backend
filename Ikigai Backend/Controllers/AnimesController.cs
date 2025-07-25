using System;
using System.Collections.Generic;
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
                    Studio = a.Studio,
                    LastUpdate = a.LastUpdate
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

            GetAnimeDTO anime = new GetAnimeDTO
            {
                Id = animeDB.Id,
                AnimeTitle = animeDB.AnimeTitle,
                Synopsis = animeDB.Synopsis,
                ReleaseDate = animeDB.ReleaseDate,
                Studio = animeDB.Studio,
                LastUpdate = animeDB.LastUpdate
            };

            return anime;
        }

        // PUT: api/Animes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutAnime(int id, UpdateAnimeDTO dto)
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
            anime.Studio = dto.Studio;
            // Optionally update LastUpdate if you want

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
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<GetAnimeDTO>> PostAnime(PostAnimeDTO animePostDTO)
        {
            if (animePostDTO == null)
            {
                return BadRequest("Anime data is null.");
            }

            var anime = new Anime
            {
                AnimeTitle = animePostDTO.AnimeTitle,
                Synopsis = animePostDTO.Synopsis,
                ReleaseDate = animePostDTO.ReleaseDate,
                Studio = animePostDTO.Studio,
                LastUpdate = animePostDTO.LastUpdate
            };

            _context.Animes.Add(anime);
            await _context.SaveChangesAsync();

            var animeGetDto = new GetAnimeDTO
            {
                Id = anime.Id,
                AnimeTitle = anime.AnimeTitle,
                Synopsis = anime.Synopsis,
                ReleaseDate = anime.ReleaseDate,
                Studio = anime.Studio,
                LastUpdate = anime.LastUpdate
            };

            return CreatedAtAction("GetAnime", new { id = animeGetDto.Id }, animeGetDto);
        }

        // DELETE: api/Animes/5
        [HttpDelete("{id}")]
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

        private bool AnimeExists(int id)
        {
            return _context.Animes.Any(e => e.Id == id);
        }
    }
}
