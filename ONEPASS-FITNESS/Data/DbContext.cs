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

            // Optionally seed a couple of upcoming sessions for testing (times in UTC)
            var now = DateTime.UtcNow.Date;
            modelBuilder.Entity<Models.ClassSession>().HasData(
                new Models.ClassSession { Id = 1, ClassTypeId = 1, StartTime = DateTime.SpecifyKind(now.AddDays(1).AddHours(9), DateTimeKind.Utc), Capacity = 12 },
                new Models.ClassSession { Id = 2, ClassTypeId = 2, StartTime = DateTime.SpecifyKind(now.AddDays(1).AddHours(18), DateTimeKind.Utc), Capacity = 15 },
                new Models.ClassSession { Id = 3, ClassTypeId = 3, StartTime = DateTime.SpecifyKind(now.AddDays(2).AddHours(7), DateTimeKind.Utc), Capacity = 10 }
            );
        }
    }
}
