using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieBestAuthorizeBased.Constant;
using MovieBestAuthorizeBased.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MovieBestAuthorizeBased.Controllers
{
    [Authorize (Roles ="SuperAdmin")]
    public class RolesController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        public RolesController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task< IActionResult >Index()
        {
            return View(await _roleManager.Roles.OrderBy(r => r.Name).ToListAsync());
        }


        [HttpPost]
        public async Task<IActionResult> CreateRole(RoleViewModel model)
        {
            if (!ModelState.IsValid)
                return View(nameof(Index), await _roleManager.Roles.OrderBy(r => r.Name).ToListAsync());

            if (await _roleManager.RoleExistsAsync(model.RoleName))
            {
                ModelState.AddModelError("RoleName", "Role is exsits");
                return View(nameof(Index), await _roleManager.Roles.OrderBy(r => r.Name).ToListAsync());
            }

            await _roleManager.CreateAsync(new IdentityRole ( model.RoleName.Trim() ));
            return View(nameof(Index), await _roleManager.Roles.OrderBy(r => r.Name).ToListAsync());

        }
//        public async Task<IActionResult> ManageUserClaims(string roleId)
//        {
//            var role = await _roleManager.FindByIdAsync(roleId);

//            if (role == null)
//                return NotFound();

//            var userClaims = _roleManager.GetClaimsAsync(role).Result.Select(c => c.Value);

//            List<CheckBoxViewModel> allPermissions = Permissions.GenerateAllPermissions().Select(p => new CheckBoxViewModel { DisplayName = p }).ToList();

//            foreach (var permission in allPermissions)
//                if (userClaims.Any(p => p == permission.DisplayName))
//                    permission.IsChecked = true;


//            PermissionsFormViewModel model = new()
//            {
//                Id = roleId,
//                Name = role.Name,
//                RoleClaims = allPermissions
//            };

//            return View(model);
//        }
//        [HttpPost]
        
//        public async Task<IActionResult> ManageUserClaims(PermissionsFormViewModel model)
//        {
//            if (!ModelState.IsValid)
//                return View(model);

//            var role = await _roleManager.FindByIdAsync(model.Id);

//            if (role == null)
//                return NotFound();

//            foreach(var claim in _roleManager.GetClaimsAsync(role).Result)
//                await _roleManager.RemoveClaimAsync(role, claim);

//            foreach(var claim in model.RoleClaims.Where(c => c.IsChecked).ToList())
//                await _roleManager.AddClaimAsync(role, new Claim("Permissions",claim.DisplayName));


//                return RedirectToAction(nameof(Index));
//;        }
    }
}
