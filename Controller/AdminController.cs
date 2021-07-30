﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SchoolTestProject.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolTestProject.Controllers
{
    [Authorize(Roles ="Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;
        public AdminController(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityRole roles = new IdentityRole
                {
                    Name = model.RoleName
                };

                IdentityResult result = await _roleManager.CreateAsync(roles);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult ListRole()
        {
            var roles = _roleManager.Roles;
            return View(roles);
        }

        [HttpGet]
        public async Task<IActionResult> EditRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return View("Notfound");
            }
            var model = new EditRoleViewModel
            {
                Id = role.Id,
                Name = role.Name
            };

            foreach (var user in _userManager.Users)
            {
                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    model.Users.Add(user.UserName);
                }
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            var role = await _roleManager.FindByIdAsync(model.Id);

            if (role == null)
            {
                return View("NotFound");
            }
            else
            {
                role.Name = model.Name;
                var result = await _roleManager.UpdateAsync(role);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListRole");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditUsersInRole(string roleId)
        {
            ViewBag.roleId = roleId;
            var role = await _roleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                return View("NotFound");
            }
            var model = new List<UserRoleViewModel>();

            foreach (var user in _userManager.Users)
            {
                var userRoleViewModel = new UserRoleViewModel()
                {

                    UserId = user.Id,
                    UserName = user.UserName
                };

                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    userRoleViewModel.IsSelected = true;
                }
                else
                {
                    userRoleViewModel.IsSelected = false;
                }
                model.Add(userRoleViewModel);
            }
            return View(model);
        }
        [HttpPost]
       public async Task<IActionResult> EditUsersInRole(List<UserRoleViewModel> model,string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                return View("NotFound");
            }

            for(int i = 0; i < model.Count; i++)
            {
                var user = await _userManager.FindByIdAsync(model[i].UserId);
                IdentityResult result = null;

                if(model[i].IsSelected && !(await _userManager.IsInRoleAsync(user, role.Name)))
                {
                    result = await _userManager.AddToRoleAsync(user,role.Name);
                }
                else if (!model[i].IsSelected && await _userManager.IsInRoleAsync(user, role.Name))
                {
                    result = await _userManager.RemoveFromRoleAsync(user, role.Name);
                }
                else
                {
                    continue;
                }
                if (result.Succeeded)
                {
                    if (i < (model.Count-1))
                    {
                        continue;
                    }
                    else
                    {
                        return RedirectToAction("EditRole",new {Id = roleId});
                    }
                }
            }
            return RedirectToAction("EditRole", new { Id = roleId });
        }

    }
}
    

