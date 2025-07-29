using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ikigai_Backend.Database;
using Ikigai_Backend.DbModels;
using Ikigai_Backend.DTOs.FollowingDTOs;
using Ikigai_Backend.DTOs.AnimeDTOs;

namespace Ikigai_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FollowingsController : ControllerBase
    {
        private readonly IkigaiDbContext _context;

        public FollowingsController(IkigaiDbContext context)
        {
            _context = context;
        }

        // GET: api/Followings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetFollowingDTO>>> GetUserFollowings()
        {
            return await _context.UserFollowings
                .Include(f => f.Anime)
                .Select(f => new GetFollowingDTO
                {
                    UserId = f.UserId,
                    AnimeId = f.AnimeId,
                    AnimeTitle = f.Anime.AnimeTitle
                })
                .ToListAsync();
        }

        // GET: api/Followings/5/10
        [HttpGet("{userId}/{animeId}")]
        public async Task<ActionResult<GetFollowingDTO>> GetFollowing(int userId, int animeId)
        {
            var following = await _context.UserFollowings
                .Include(f => f.Anime)
                .Where(f => f.UserId == userId && f.AnimeId == animeId)
                .Select(f => new GetFollowingDTO
                {
                    UserId = f.UserId,
                    AnimeId = f.AnimeId,
                    AnimeTitle = f.Anime.AnimeTitle
                })
                .FirstOrDefaultAsync();

            if (following == null)
            {
                return NotFound();
            }

            return following;
        }

        // GET: api/Followings/user/5/animes
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<GetAnimeDTO>>> GetFollowedAnimesByUser(int userId)
        {
            var animes = await _context.UserFollowings
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

        // POST: api/Followings
        [HttpPost]
        public async Task<ActionResult<GetFollowingDTO>> PostFollowing(PostFollowingDTO followingDto)
        {
            var following = new Following
            {
                UserId = followingDto.UserId,
                AnimeId = followingDto.AnimeId
            };

            _context.UserFollowings.Add(following);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (FollowingExists(followingDto.UserId, followingDto.AnimeId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            // Return the created following with anime title
            var created = await _context.UserFollowings
                .Include(f => f.Anime)
                .Where(f => f.UserId == followingDto.UserId && f.AnimeId == followingDto.AnimeId)
                .Select(f => new GetFollowingDTO
                {
                    UserId = f.UserId,
                    AnimeId = f.AnimeId,
                    AnimeTitle = f.Anime.AnimeTitle
                })
                .FirstOrDefaultAsync();

            return CreatedAtAction(nameof(GetFollowing), new { userId = followingDto.UserId, animeId = followingDto.AnimeId }, created);
        }

        // DELETE: api/Followings/5/10
        [HttpDelete("{userId}/{animeId}")]
        public async Task<IActionResult> DeleteFollowing(int userId, int animeId)
        {
            var following = await _context.UserFollowings.FindAsync(userId, animeId);
            if (following == null)
            {
                return NotFound();
            }

            _context.UserFollowings.Remove(following);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FollowingExists(int userId, int animeId)
        {
            return _context.UserFollowings.Any(e => e.UserId == userId && e.AnimeId == animeId);
        }
    }
}