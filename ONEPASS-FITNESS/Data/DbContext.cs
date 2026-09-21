using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ONEPASS_FITNESS.Areas.Identity.Pages;
using ONEPASS_FITNESS.Models;

namespace ONEPASS_FITNESS.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

 
        public DbSet<Classes> Classes { get; set; }
        public DbSet<ClassBookings> ClassBookings { get; set; }
        public DbSet<Models.ClassType> ClassTypes { get; set; }
        public DbSet<Models.ClassSession> ClassSessions { get; set; }
        public DbSet<Models.Booking> Bookings { get; set; }
        public DbSet<Progress> Progress { get; set; }
        public DbSet<WeightEntry> WeightEntries { get; set; }
        public DbSet<WeightGoal> WeightGoals { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed basic class types so they always exist
            modelBuilder.Entity<Models.ClassType>().HasData(
                new Models.ClassType { Id = 1, Name = "HIIT", Description = "High intensity interval training", DurationMinutes = 45, IsActive = true },
                new Models.ClassType { Id = 2, Name = "Yoga", Description = "Vinyasa flow and stretching", DurationMinutes = 60, IsActive = true },
                new Models.ClassType { Id = 3, Name = "Pilates", Description = "Mat Pilates core and mobility", DurationMinutes = 50, IsActive = true }
            );

            // Unique index to prevent duplicate bookings for same user/session
            modelBuilder.Entity<Models.Booking>()
                .HasIndex(b => new { b.ClassSessionId, b.UserId })
                .IsUnique();

        }
    }
}
