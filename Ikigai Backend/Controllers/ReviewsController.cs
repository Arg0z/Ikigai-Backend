using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ikigai_Backend.Database;
using Ikigai_Backend.DbModels;
using Ikigai_Backend.DTOs.ReviewDTOs;

namespace Ikigai_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly IkigaiDbContext _context;

        public ReviewsController(IkigaiDbContext context)
        {
            _context = context;
        }

        // GET: api/Reviews
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetReviewDTO>>> GetReviews()
        {
            return await _context.Reviews
                .Include(r => r.User)
                .Select(r => new GetReviewDTO
                {
                    UserId = r.UserId,
                    UserName = r.User.Name,
                    AnimeId = r.AnimeId,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt
                })
                .ToListAsync();
        }

        // GET: api/Reviews/5/10
        [HttpGet("{userId}/{animeId}")]
        public async Task<ActionResult<GetReviewDTO>> GetReview(int userId, int animeId)
        {
            var review = await _context.Reviews
                .Include(r => r.User)
                .Where(r => r.UserId == userId && r.AnimeId == animeId)
                .Select(r => new GetReviewDTO
                {
                    UserId = r.UserId,
                    UserName = r.User.Name,
                    AnimeId = r.AnimeId,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt
                })
                .FirstOrDefaultAsync();

            if (review == null)
                return NotFound();

            return review;
        }

        // GET: api/Reviews/anime/10
        [HttpGet("anime/{animeId}")]
        public async Task<ActionResult<IEnumerable<GetReviewDTO>>> GetReviewsByAnime(int animeId)
        {
            return await _context.Reviews
                .Where(r => r.AnimeId == animeId)
                .Include(r => r.User)
                .Select(r => new GetReviewDTO
                {
                    UserId = r.UserId,
                    UserName = r.User.Name,
                    AnimeId = r.AnimeId,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt
                })
                .ToListAsync();
        }

        // GET: api/Reviews/anime/10/average-rating
        [HttpGet("anime/{animeId}/average-rating")]
        public async Task<ActionResult<double>> GetAverageRating(int animeId)
        {
            var avg = await _context.Reviews
                .Where(r => r.AnimeId == animeId)
                .AverageAsync(r => (double?)r.Rating) ?? 0.0;
            return avg;
        }

        // POST: api/Reviews
        [HttpPost]
        public async Task<ActionResult<GetReviewDTO>> PostReview(PostReviewDTO dto)
        {
            var review = new Review
            {
                UserId = dto.UserId,
                AnimeId = dto.AnimeId,
                Rating = dto.Rating,
                Comment = dto.Comment,
                CreatedAt = DateTime.UtcNow
            };

            _context.Reviews.Add(review);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ReviewExists(dto.UserId, dto.AnimeId))
                    return Conflict();
                throw;
            }

            var created = await _context.Reviews
                .Include(r => r.User)
                .Where(r => r.UserId == dto.UserId && r.AnimeId == dto.AnimeId)
                .Select(r => new GetReviewDTO
                {
                    UserId = r.UserId,
                    UserName = r.User.Name,
                    AnimeId = r.AnimeId,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt
                })
                .FirstOrDefaultAsync();

            return CreatedAtAction(nameof(GetReview), new { userId = dto.UserId, animeId = dto.AnimeId }, created);
        }

        // PUT: api/Reviews/5/10
        [HttpPut("{userId}/{animeId}")]
        public async Task<IActionResult> PutReview(int userId, int animeId, PutReviewDTO dto)
        {
            if (userId != dto.UserId || animeId != dto.AnimeId)
                return BadRequest();

            var review = await _context.Reviews.FindAsync(userId, animeId);
            if (review == null)
                return NotFound();

            review.Rating = dto.Rating;
            review.Comment = dto.Comment;

            _context.Entry(review).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Reviews/5/10
        [HttpDelete("{userId}/{animeId}")]
        public async Task<IActionResult> DeleteReview(int userId, int animeId)
        {
            var review = await _context.Reviews.FindAsync(userId, animeId);
            if (review == null)
                return NotFound();

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ReviewExists(int userId, int animeId)
        {
            return _context.Reviews.Any(e => e.UserId == userId && e.AnimeId == animeId);
        }
    }
}
