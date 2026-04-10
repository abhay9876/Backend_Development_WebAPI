using Microsoft.EntityFrameworkCore;
using TaskManager.Models.Entity;
using TaskM = TaskManager.Models.Entity.TaskM;

namespace TaskManager.Repository.Context
{
    public class TaskManagerDbContext : DbContext
    {

        public TaskManagerDbContext(DbContextOptions<TaskManagerDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<TaskM> Tasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.UserName)
                .IsUnique();

            //modelBuilder.Entity<TaskM>()
            //    .HasOne(t => t.User)
            //    .WithMany()
            //    .HasForeignKey(t => t.UserId)
            //    .OnDelete(DeleteBehavior.Cascade);
        }

    }
}
