using Microsoft.EntityFrameworkCore;
using FundooNotes.Models.Entity;
namespace FundooNotes.Repository.Context
{
    public class AppDbContext : DbContext 
    {
        // Constructor jo options accept karta hai (connection string etc.)
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Note> Notes { get; set; }

       

        // Fluent API se additional configuration kar sakte hain (optional)
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Unique constraint on Email
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();


            
        }
    }
}
