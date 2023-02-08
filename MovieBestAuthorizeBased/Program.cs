using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MovieBestAuthorizeBased.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieBestAuthorizeBased
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            IHost host = CreateHostBuilder(args).Build();

            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;
            var iLoggerFactory = services.GetRequiredService<ILoggerProvider>();
            var logger = iLoggerFactory.CreateLogger("app");

            try
            {
                var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                await Seeds.DefaultRolesSeeds.SeedDefaultRoles(roleManager);
                await Seeds.DefaultUsersSeeds.CreateDefaultAdmin(userManager,roleManager);
                await Seeds.DefaultUsersSeeds.CreateDefaultUser(userManager);

                logger.LogInformation("Data seeded");
                logger.LogInformation("Application Started");
            }
            catch(Exception exc)
            {
                logger.LogWarning(exc, "An error occurred while seeding data");
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
