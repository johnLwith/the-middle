using Microsoft.EntityFrameworkCore;
using WebAppp.Models;

namespace WebAppp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Episode> Episodes { get; set; }
        public DbSet<Word> Words { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Episode>()
                .ToTable("episodes", "public");

            modelBuilder.Entity<Word>()
                .ToTable("words", "public");
        }
    }
}