using Microsoft.AspNetCore.Identity;
using MovieBestAuthorizeBased.Constant;
using MovieBestAuthorizeBased.Models;
using System.Linq;
using System.Threading.Tasks;

namespace MovieBestAuthorizeBased.Seeds
{
    public static class DefaultRolesSeeds
    {
        public static async Task SeedDefaultRoles(RoleManager<IdentityRole> roleManager)
        {
            if (!roleManager.Roles.Any())
            {
                await roleManager.CreateAsync(new IdentityRole(DefaultRoles.Admin.ToString()));
                await roleManager.CreateAsync(new IdentityRole(DefaultRoles.User.ToString()));
                await roleManager.CreateAsync(new IdentityRole(DefaultRoles.SuperAdmin.ToString()));
            }
            
        }
    }
}
