using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace ONEPASS_FITNESS.Data
{
    // This class is responsible for seeding the database with an initial admin user and role.
    //Login details for the admin user are read from the configuration (appsettings.json or environment variables).
    public static class DbSeeder
    {
        public static async Task SeedAdminAsync(IServiceProvider services, IConfiguration config)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<Areas.Identity.Pages.AppUser>>();

            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                var r = await roleManager.CreateAsync(new IdentityRole("Admin"));
                if (!r.Succeeded)
                    logger.LogWarning("Failed to create Admin role: {Errors}", string.Join(", ", r.Errors.Select(e => e.Description)));
            }

            var email = config["Admin:Email"];
            var password = config["Admin:Password"];
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                logger.LogInformation("Admin credentials not provided in configuration (Admin:Email / Admin:Password). Skipping admin seed.");
                return; //nothing to do without credentials
            }

            var admin = await userManager.FindByEmailAsync(email);
            if (admin == null)
            {
                admin = new Areas.Identity.Pages.AppUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                    //fill required fields on AppUser so other pages don't fail
                    Name = "Admin",
                    Lastname = "User",
                    DOB = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-30)),
                    PhoneNumber = "0000000000"
                };

                var result = await userManager.CreateAsync(admin, password);
                if (!result.Succeeded)
                {
                    logger.LogError("Admin seed failed: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
                    return;
                }
            }
            //Without credentials in settings theres nothing to create or update, so we can skip the rest of the method if the admin user already exists and credentials are not provided.
            else if (!await userManager.CheckPasswordAsync(admin, password))
            {
                var token = await userManager.GeneratePasswordResetTokenAsync(admin);
                var reset = await userManager.ResetPasswordAsync(admin, token, password);
                if (!reset.Succeeded)
                    logger.LogWarning("Failed to reset admin password: {Errors}", string.Join(", ", reset.Errors.Select(e => e.Description)));
            }

            if (!await userManager.IsInRoleAsync(admin, "Admin"))
            {
                var addRoleResult = await userManager.AddToRoleAsync(admin, "Admin");
                if (!addRoleResult.Succeeded)
                    logger.LogWarning("Failed to add admin user to Admin role: {Errors}", string.Join(", ", addRoleResult.Errors.Select(e => e.Description)));
            }
        }
    }
}
