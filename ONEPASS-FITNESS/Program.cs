using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ONEPASS_FITNESS.Areas.Identity.Pages;
using ONEPASS_FITNESS.Data;

namespace ONEPASS_FITNESS
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddDefaultIdentity<AppUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
            })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.AddControllersWithViews();

            // Register a TimeZoneInfo for class times (configure Gym:TimeZone in appsettings or user-secrets)
            var tzId = builder.Configuration["Gym:TimeZone"] ?? "UTC";
            builder.Services.AddSingleton(TimeZoneInfo.FindSystemTimeZoneById(tzId));

            builder.Services.AddAuthorization(o =>
                o.AddPolicy("AdminOnly", p => p.RequireRole("Admin")));

            builder.Services.AddRazorPages(options =>
            {
                // Protect the admin folder with the AdminOnly policy
                options.Conventions.AuthorizeFolder("/Admin", "AdminOnly");
            });

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.MapRazorPages();

            // Seed admin user/roles at startup (reads credentials from configuration / user-secrets)
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<ApplicationDbContext>();
                var userManager = services.GetRequiredService<UserManager<AppUser>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                // Apply any pending EF migrations so new schema is available on first run
                try
                {
                    context.Database.Migrate();
                }
                catch (Exception ex)
                {

                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "Database migrate failed at startup");
                }

                try
                {
                    DbInitializer.Initialize(context, userManager, roleManager);
                }
                catch { /* swallow - keep startup resilient */ }

                try
                {
                    var config = services.GetRequiredService<Microsoft.Extensions.Configuration.IConfiguration>();
                    await DbSeeder.SeedAdminAsync(services, config);
                }
                catch { /* swallow - seeding optional */ }
            }

            app.Run();
        }
    }
}
