using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ikigai_Backend.Database;
using Ikigai_Backend.DbModels;
using Ikigai_Backend.DTOs.FavouriteDTOs;
using Ikigai_Backend.DTOs.AnimeDTOs;

namespace Ikigai_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FavouritesController : ControllerBase
    {
        private readonly IkigaiDbContext _context;

        public FavouritesController(IkigaiDbContext context)
        {
            _context = context;
        }

        // GET: api/Favourites
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetFavouriteDTO>>> GetUserFavourites()
        {
            return await _context.UserFavourites
                .Include(f => f.Anime)
                .Select(f => new GetFavouriteDTO
                {
                    UserId = f.UserId,
                    AnimeId = f.AnimeId,
                    AnimeTitle = f.Anime.AnimeTitle
                })
                .ToListAsync();
        }

        // GET: api/Favourites/5/10
        [HttpGet("{userId}/{animeId}")]
        public async Task<ActionResult<GetFavouriteDTO>> GetFavourite(int userId, int animeId)
        {
            var favourite = await _context.UserFavourites
                .Include(f => f.Anime)
                .Where(f => f.UserId == userId && f.AnimeId == animeId)
                .Select(f => new GetFavouriteDTO
                {
                    UserId = f.UserId,
                    AnimeId = f.AnimeId,
                    AnimeTitle = f.Anime.AnimeTitle
                })
                .FirstOrDefaultAsync();

            if (favourite == null)
            {
                return NotFound();
            }

            return favourite;
        }

        // GET: api/Favourites/user/{userId}/animes
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<GetAnimeDTO>>> GetFavouriteAnimesByUser(int userId)
        {
            var animes = await _context.UserFavourites
                .Where(f => f.UserId == userId)
                .Include(f => f.Anime)
                .Select(f => new GetAnimeDTO
                {
                    Id = f.Anime.Id,
                    AnimeTitle = f.Anime.AnimeTitle,
                    Synopsis = f.Anime.Synopsis,
                    ReleaseDate = f.Anime.ReleaseDate,
                    Studio = f.Anime.Studio,
                    LastUpdate = f.Anime.LastUpdate
                })
                .ToListAsync();

            return animes;
        }

        // POST: api/Favourites
        [HttpPost]
        public async Task<ActionResult<GetFavouriteDTO>> PostFavourite(PostFavouriteDTO favouriteDto)
        {
            var favourite = new Favourite
            {
                UserId = favouriteDto.UserId,
                AnimeId = favouriteDto.AnimeId
            };

            _context.UserFavourites.Add(favourite);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (FavouriteExists(favouriteDto.UserId, favouriteDto.AnimeId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            // Return the created favourite with anime title
            var created = await _context.UserFavourites
                .Include(f => f.Anime)
                .Where(f => f.UserId == favouriteDto.UserId && f.AnimeId == favouriteDto.AnimeId)
                .Select(f => new GetFavouriteDTO
                {
                    UserId = f.UserId,
                    AnimeId = f.AnimeId,
                    AnimeTitle = f.Anime.AnimeTitle
                })
                .FirstOrDefaultAsync();

            return CreatedAtAction(nameof(GetFavourite), new { userId = favouriteDto.UserId, animeId = favouriteDto.AnimeId }, created);
        }

        // DELETE: api/Favourites/5/10
        [HttpDelete("{userId}/{animeId}")]
        public async Task<IActionResult> DeleteFavourite(int userId, int animeId)
        {
            var favourite = await _context.UserFavourites.FindAsync(userId, animeId);
            if (favourite == null)
            {
                return NotFound();
            }

            _context.UserFavourites.Remove(favourite);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FavouriteExists(int userId, int animeId)
        {
            return _context.UserFavourites.Any(e => e.UserId == userId && e.AnimeId == animeId);
        }
    }
}
