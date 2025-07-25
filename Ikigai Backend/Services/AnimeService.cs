using System;
using System.Threading.Tasks;
using Ikigai_Backend.Database;
using Microsoft.EntityFrameworkCore;

//Shared Class for work between the different contrllers(eg AnimesController, EpisodesController, etc.)
namespace Ikigai_Backend.Services
{
    public class AnimeService
    {
        private readonly IkigaiDbContext _context;

        public AnimeService(IkigaiDbContext context)
        {
            _context = context;
        }

        //Update LastUpdate property of the Anime model. Is called from the EpisodesController when new episodes are added.
        public async Task<bool> UpdateAnimeLastUpdateAsync(int animeId)
        {
            var anime = await _context.Animes.FindAsync(animeId);
            if (anime == null)
                return false;

            anime.LastUpdate = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}