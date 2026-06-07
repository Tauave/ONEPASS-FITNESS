using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ONEPASS_FITNESS.Models;

namespace ONEPASS_FITNESS.Data
{
    public class DbInitializer
    {
        public static void Initialize(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            context.Database.Migrate();

            if (context.Classes.Any())
            {
                return;
            }

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

            var personalInfos = new Personalinfo[]
            {
                new Personalinfo { Name = "John", Lastname = "Doe", DOB = DateOnly.Parse("1990-01-01"), Email = "john.doe@example.com", PhoneNumber = "0224651243" },
                new Personalinfo { Name = "Jane", Lastname = "Smith", DOB = DateOnly.Parse("1992-03-15"), Email = "jane.smith@example.com", PhoneNumber = "0211111111" },
                new Personalinfo { Name = "Michael", Lastname = "Brown", DOB = DateOnly.Parse("1988-07-22"), Email = "michael.brown@example.com", PhoneNumber = "0212222222" },
                new Personalinfo { Name = "Olivia", Lastname = "Wilson", DOB = DateOnly.Parse("1994-11-05"), Email = "olivia.wilson@example.com", PhoneNumber = "0213333333" },
                new Personalinfo { Name = "Liam", Lastname = "Taylor", DOB = DateOnly.Parse("1991-02-10"), Email = "liam.taylor@example.com", PhoneNumber = "0214444444" }
            };

            for (var i = 0; i < personalInfos.Length; i++)
            {
                var profile = personalInfos[i];
                var user = userManager.FindByEmailAsync(profile.Email).GetAwaiter().GetResult();

                if (user == null)
                {
                    user = new IdentityUser
                    {
                        UserName = profile.Email,
                        Email = profile.Email,
                        EmailConfirmed = true
                    };

                    userManager.CreateAsync(user, "Test1234").GetAwaiter().GetResult();
                    userManager.AddToRoleAsync(user, "Member").GetAwaiter().GetResult();
                }

                profile.IdentityUserId = user.Id;
            }

            context.Personalinfos.AddRange(personalInfos);
            context.SaveChanges();

            var classes = new Classes[]
            {
                new Classes { Classname = "Yoga", Date = DateOnly.Parse("2026-05-01"), Starttime = TimeOnly.Parse("09:00"), Endtime = TimeOnly.Parse("10:00"), Capacity = 20 },
                new Classes { Classname = "HIIT", Date = DateOnly.Parse("2026-05-02"), Starttime = TimeOnly.Parse("18:00"), Endtime = TimeOnly.Parse("19:00"), Capacity = 15 },
                new Classes { Classname = "Pilates", Date = DateOnly.Parse("2026-05-03"), Starttime = TimeOnly.Parse("11:00"), Endtime = TimeOnly.Parse("12:00"), Capacity = 10 }
            };

            context.Classes.AddRange(classes);
            context.SaveChanges();

            var progress = new Progress[]
            {
                new Progress { Personalinfoid = personalInfos[0].PersonalinfoId, Weight = 82.5m, DateRecorded = DateOnly.Parse("2026-05-01") },
                new Progress { Personalinfoid = personalInfos[1].PersonalinfoId, Weight = 68.0m, DateRecorded = DateOnly.Parse("2026-05-01") }
            };

            context.Progress.AddRange(progress);
            context.SaveChanges();
        }
    }
}
