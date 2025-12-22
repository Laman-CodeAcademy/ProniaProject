using Microsoft.EntityFrameworkCore;
using Pronia.Models; 

namespace Pronia.Contexts
{
    public class AppDbContext : DbContext
    {

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Card> Cards { get; set; }
    }
}
