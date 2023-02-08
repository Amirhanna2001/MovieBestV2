using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieBestAuthorizeBased.Models;
using MovieBestAuthorizeBased.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using System;

namespace MovieBestAuthorizeBased.Controllers
{
    [Authorize(Roles =("SuperAdmin"))]
    public class UserController : Controller
    {
        private RoleManager<IdentityRole> _roleManager;
        private UserManager<ApplicationUser> _userManager;
        public UserController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }
        public async Task< IActionResult> Index()
        {
            List<UserViewModel> users = await _userManager.Users.Select(user => new UserViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName,
                Email = user.Email,
                UserRoles = _userManager.GetRolesAsync(user).Result
            }).ToListAsync();

            //var roles = await _userManager.GetRolesAsync(users).ToListAsync();

            return View(users);
        }
        
        public async Task<IActionResult> AddNewUser()
        {
            List<CheckBoxViewModel> roles = await _roleManager.Roles.Select(r =>
                        new CheckBoxViewModel {  DisplayName = r.Name })
                        .ToListAsync();

            AddUserViewModel user = new AddUserViewModel
            {
                Roles = roles
            };
            return View(user);
        }
        [HttpPost]
        public async Task<IActionResult> AddNewUser(AddUserViewModel model)
        {
            if(!ModelState.IsValid)
                return View(model);
            if(await _userManager.FindByEmailAsync(model.Email) != null)
            {
                ModelState.AddModelError("Email", $"Email {model.Email} is already exists" );
                return View(model);
            }
            if (await _userManager.FindByNameAsync(model.UserName) != null)
            {
                ModelState.AddModelError("UserName", $"Username {model.UserName} is already exists");
                return View(model);
            }
                
            if(!model.Roles.Any(r => r.IsChecked))
            {
                ModelState.AddModelError("Roles", "You should select at least one role");
                return View(model);
            }
            ApplicationUser user = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                
            };
            var result = await _userManager.CreateAsync(user,model.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("Roles", error.Description);

                return View(model);
            }
            await _userManager.AddToRolesAsync(user, model.Roles.Where(r => r.IsChecked).Select(r => r.DisplayName));
                
            
            return RedirectToAction(nameof(Index));

        }
        public async Task<IActionResult> ManageUser(string id)
        { 
            ApplicationUser user = await _userManager.FindByIdAsync(id);

            if(user == null)
                return NotFound();

            var userRoles = await _roleManager.Roles.ToListAsync();
            ManageUserViewModel viewModel = new ManageUserViewModel
            {
                UserId = user.Id,
                UserName = user.UserName,
                LastName = user.LastName,
                FirstName = user.FirstName,
                Email = user.Email,
                UserRoles = userRoles.Select(role => new CheckBoxViewModel
                {
                    DisplayName = role.Name,
                    IsChecked = _userManager.IsInRoleAsync(user,role.Name).Result
                }).ToList()
            };
            return View(viewModel);

        }
        [HttpPost]
        public async Task<IActionResult> ManageUser(ManageUserViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            

            ApplicationUser user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
                return NotFound();


            ApplicationUser userByEmail = await _userManager.FindByEmailAsync(model.Email);
            
            var userRoles = await _userManager.GetRolesAsync(user);
            
            if(userByEmail != null && userByEmail.Id != model.UserId)
            {
                ModelState.AddModelError("Email", $"{model.Email} is Already exists to another user");
                return View(model);
            }
            ApplicationUser userByUsername = await _userManager.FindByNameAsync(model.UserName);
            if(userByUsername != null && userByUsername.Id != model.UserId)
            {
                ModelState.AddModelError("UserName", $"{model.UserName} is Already exists to another user");
                return View(model);
            }
            await _userManager.RemoveFromRolesAsync(user, userRoles);
            await _userManager.AddToRolesAsync(user,model.UserRoles.Where(r => r.IsChecked).Select(r => r.DisplayName));
            //Another Method
            //foreach (var role in model.UserRoles)
            //{
            //    if (userRoles.Any(r => r == role.RoleName) && !role.IsChecked)
            //        await _userManager.RemoveFromRoleAsync(userByEmail, role.RoleName);

            //    if (!userRoles.Any(r => r == role.RoleName) && role.IsChecked)
            //        await _userManager.AddToRoleAsync(userByEmail, role.RoleName);
            //}
            user.Email = model.Email;
            user.UserName = model.UserName;
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;

            await _userManager.UpdateAsync(user);

            return RedirectToAction(nameof(Index));

        }
        public async Task<IActionResult> Delete(string id)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
                throw new Exception();

            return RedirectToAction(nameof(Index));
        }
    }
}
