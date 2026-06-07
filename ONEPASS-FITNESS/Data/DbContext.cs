using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ONEPASS_FITNESS.Models;

namespace ONEPASS_FITNESS.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<PersonalInfo> PersonalInfos { get; set; }
        public DbSet<Classes> Classes { get; set; }
        public DbSet<ClassBookings> ClassBookings { get; set; }
        public DbSet<Progress> Progress { get; set; }
    }
}