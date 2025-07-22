using Ikigai_Backend.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace Ikigai_Backend.Database
{
    public class IkigaiDbContext : DbContext
    {
        public IkigaiDbContext(DbContextOptions<IkigaiDbContext> options)
       : base(options) { }

        public DbSet<User> Users => Set<User>();
    }
}
