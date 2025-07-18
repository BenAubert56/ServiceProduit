using Microsoft.EntityFrameworkCore;

namespace ServiceCommentaire.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Comment> Comments => Set<Comment>();
    }
}
