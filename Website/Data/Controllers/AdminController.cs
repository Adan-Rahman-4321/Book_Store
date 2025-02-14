using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;
using Website.Models;
using Website.ViewModel;

namespace Website.Controllers
{
    [Authorize(Roles ="Admin")]
    public class AdminController : Controller
    {

        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<DefaultUser> _userManager;
        public AdminController(RoleManager<IdentityRole> roleManager , UserManager<DefaultUser> userManager)

		{
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task<IActionResult> UsersList()
        {
            var usersQuery = _userManager.Users.AsQueryable();

            var users = await usersQuery.ToListAsync();


            return View(users);
        }

        [HttpGet]
        public IActionResult ListAllRoles()
        {
            var roles = _roleManager.Roles;
            return View(roles);
        }

        [HttpGet]
        public IActionResult AddRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddRole(AddRoleVM model)
        {
            if (ModelState.IsValid)
            {
                IdentityRole identityRole = new()
                {
                    Name = model.RoleName
                };

                var result = await _roleManager.CreateAsync(identityRole);
                if (result.Succeeded)
                {
                    return RedirectToAction("ListAllRoles");
                }

                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);

            if(id == null)
            {
                ViewData["ErrorMessage"] = $@"No role with id '{id}' was found";
                return View("Error");
            }

            EditRoleVM model = new()
            {
                Id = role.Id,
                RoleName = role.Name,
            };

            var Users = await _userManager.Users.ToListAsync();

            foreach(var user in Users)
            {
                if(await _userManager.IsInRoleAsync(user , role.Name))
                {
                    model.Users.Add(user.UserName);
                }
            }

            return View(model);
        }



        [HttpPost]
        public async Task<IActionResult> EditRole(EditRoleVM model)
        {
            var role = await _roleManager.FindByIdAsync(model.Id);

            if (role == null)
            {
                ViewData["ErrorMessage"] = $@"No role with id '{model.Id}' was found";
                return View("Error");
            }
            else
            {
                role.Name = model.RoleName;

                var result = await _roleManager.UpdateAsync(role);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListAllRoles");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View(model);
            }
		}


        [HttpGet]
        public async Task<IActionResult> EditUsersInRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);

            ViewData["roleId"] = id;
            ViewData["roleName"] = role.Name;

            if (role == null) 
            {
                ViewData["ErrorMessage"] = $"No role with Id '{id}' was found";
                return View("Error");
            }

            var model = new List<UserRoleVM>();

            var users = await _userManager.Users.ToListAsync();

            foreach(var user in users)
            {
                UserRoleVM userRoleVM = new()
                {
                    Id = user.Id,
                    Name = user.UserName
                };

                if(await _userManager.IsInRoleAsync(user , role.Name))
                {
                    userRoleVM.IsSelected = true;
                }
                else
                {
                    userRoleVM.IsSelected = false;
                }

                model.Add(userRoleVM);

            }

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> EditUsersInRole(List<UserRoleVM> model, string id)
        {
            var role = await _roleManager.FindByIdAsync(id);

            if (role == null)
            {
                ViewData["ErrorMessage"] = $"No role with Id '{id}' was found";
                return View("Error");
            }

            for (int i = 0; i < model.Count; i++)
            {
                var user = await _userManager.FindByIdAsync(model[i].Id);

                if (model[i].IsSelected && !(await _userManager.IsInRoleAsync(user, role.Name)))
                {
                    await _userManager.AddToRoleAsync(user, role.Name);
                }
                else if (!model[i].IsSelected && await _userManager.IsInRoleAsync(user, role.Name))
                {
                    await _userManager.RemoveFromRoleAsync(user, role.Name);
                }
                else
                {
                    continue;
                }
            }

            return RedirectToAction("EditRole", new { Id = id });
        }


        [HttpGet]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);

            return View(role);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmDelete( string id)
        {
            var role = await _roleManager.FindByIdAsync(id);

			if (role == null)
			{
				ViewData["ErrorMessage"] = $"No role with Id '{id}' was found";
				return View("Error");
			}
            else
            {
                var result = await _roleManager.DeleteAsync(role);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListAllRoles");
                }

                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View(role);
            }
		}
    }
}
