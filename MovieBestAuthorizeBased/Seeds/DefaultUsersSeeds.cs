using Microsoft.AspNetCore.Identity;
using MovieBestAuthorizeBased.Constant;
using MovieBestAuthorizeBased.Models;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MovieBestAuthorizeBased.Seeds
{
    public static class DefaultUsersSeeds
    {
        public static async Task CreateDefaultUser(UserManager<ApplicationUser> userManager)
        {
            ApplicationUser defaultUser = new ApplicationUser
            {
                FirstName ="Basic",
                LastName  ="User",
                UserName  ="BasicUser",
                Email = "BasicUser@MovieBest.com",
                EmailConfirmed = true,
            };
            ApplicationUser user = await userManager.FindByEmailAsync(defaultUser.Email);
            if(user == null)
            {
                await userManager.CreateAsync(defaultUser,"115P@ssW0rd130");
                await userManager.AddToRoleAsync(defaultUser,DefaultRoles.User.ToString());
            }
        }
        public static async Task CreateDefaultAdmin(UserManager<ApplicationUser> userManager,
                                                     RoleManager<IdentityRole> roleManager)
        {
            ApplicationUser defaultUser = new()
            {
                FirstName ="SuperAdmin",
                LastName  ="SuperAdmin",
                UserName  ="SuperAdmin",
                Email = "SuperAdmin@MovieBest.com",
                EmailConfirmed = true,
            };
            ApplicationUser user = await userManager.FindByEmailAsync(defaultUser.Email);
            if(user == null)
            {
                await userManager.CreateAsync(defaultUser,"115P@ssW0rd130");
                await userManager.AddToRolesAsync(defaultUser,new List<string> { 
                    DefaultRoles.SuperAdmin.ToString(),
                    DefaultRoles.User.ToString() , 
                    DefaultRoles.Admin.ToString() });
            }
            await roleManager.SeedClaimsForAdminUser();

        }
        private static async Task SeedClaimsForAdminUser(this RoleManager<IdentityRole> roleManager)
        {
            var adminRole = await roleManager.FindByNameAsync(DefaultRoles.SuperAdmin.ToString());
            await roleManager.AddPermissionClaims(adminRole,"Products");
        }
        public static async Task AddPermissionClaims(this RoleManager<IdentityRole> roleManager, IdentityRole role, string module)
        {
            var allClaims = await roleManager.GetClaimsAsync(role);
            var allPermissions = Permissions.GeneratePermissionsList(module);

            foreach (var permission in allPermissions)
            {
                if (!allClaims.Any(c => c.Type == "Permission" && c.Value == permission))
                    await roleManager.AddClaimAsync(role, new Claim("Permission", permission));
            }
        }
    }
}
