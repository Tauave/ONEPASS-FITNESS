using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ONEPASS_FITNESS.Models;

namespace ONEPASS_FITNESS.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Personalinfo> Personalinfos { get; set; }
        public DbSet<Classes> Classes { get; set; }
        public DbSet<ClassBookings> ClassBookings { get; set; }
        public DbSet<Progress> Progress { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Personalinfo>(entity =>
            {
                entity.ToTable("Personalinfo");

                entity.HasKey(p => p.PersonalinfoId);

                entity.HasIndex(p => p.IdentityUserId).IsUnique();

                entity.HasOne<IdentityUser>()
                    .WithOne()
                    .HasForeignKey<Personalinfo>(p => p.IdentityUserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Classes>(entity =>
            {
                entity.HasKey(c => c.Classid);
            });

            modelBuilder.Entity<ClassBookings>(entity =>
            {
                entity.HasKey(b => b.BookingID);

                entity.HasOne(b => b.Class)
                    .WithMany(c => c.ClassBookings)
                    .HasForeignKey(b => b.Classid)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(b => b.Personalinfo)
                    .WithMany(p => p.ClassBookings)
                    .HasForeignKey(b => b.Personalinfoid)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Progress>(entity =>
            {
                entity.HasKey(p => p.ProgressId);

                entity.Property(p => p.Weight).HasPrecision(6, 2);

                entity.HasOne(p => p.Personalinfo)
                    .WithMany(pi => pi.Progress)
                    .HasForeignKey(p => p.Personalinfoid)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
