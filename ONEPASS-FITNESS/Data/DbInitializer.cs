using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ONEPASS_FITNESS.Areas.Identity.Pages;
using ONEPASS_FITNESS.Models;

namespace ONEPASS_FITNESS.Data
{
    public class DbInitializer
    {
        private const string StaffPassword = "Staff1234";

        public static void Initialize(
            ApplicationDbContext context,
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            context.Database.Migrate();

            var roles = new IdentityRole[]
            {
                new IdentityRole { Name = "Admin" },
                new IdentityRole { Name = "Trainer" },
                new IdentityRole { Name = "Member" }
            };

            foreach (var role in roles)
            {
                if (!roleManager.RoleExistsAsync(role.Name!).GetAwaiter().GetResult())
                {
                    roleManager.CreateAsync(role).GetAwaiter().GetResult();
                }
            }

         
            if (!context.Classes.Any())
            {
                var classes = new Classes[]
                {
                    new Classes { Classname = "Yoga", Date = DateOnly.Parse("2026-05-01"), Starttime = TimeOnly.Parse("09:00"), Endtime = TimeOnly.Parse("10:00"), Capacity = 20 },
                    new Classes { Classname = "HIIT", Date = DateOnly.Parse("2026-05-02"), Starttime = TimeOnly.Parse("18:00"), Endtime = TimeOnly.Parse("19:00"), Capacity = 15 },
                    new Classes { Classname = "Pilates", Date = DateOnly.Parse("2026-05-03"), Starttime = TimeOnly.Parse("11:00"), Endtime = TimeOnly.Parse("12:00"), Capacity = 10 }
                };

                context.Classes.AddRange(classes);
                context.SaveChanges();
            }
        }

        private static void SeedStaffUser(
            UserManager<IdentityUser> userManager,
            ApplicationDbContext context,
            string email,
            string password,
            string[] roles,
            string name,
            string lastname,
            string phone)
        {
            var user = userManager.FindByEmailAsync(email).GetAwaiter().GetResult();
            if (user == null)
            {
                user = new IdentityUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };

                userManager.CreateAsync(user, password).GetAwaiter().GetResult();
            }

            foreach (var role in roles)
            {
                if (!userManager.IsInRoleAsync(user, role).GetAwaiter().GetResult())
                {
                    userManager.AddToRoleAsync(user, role).GetAwaiter().GetResult();
                }
            }

            
        }
    }
}
