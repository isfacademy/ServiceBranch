using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ServiceBranch.Models.ViewModel;

namespace ServiceBranch.Controllers
{
    [Authorize(Roles="SuperAdmin")]
    public class AdministrationController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;
        public AdministrationController(RoleManager<IdentityRole> roleManager , UserManager<IdentityUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ListOfRoles()
        {
            var roles = _roleManager.Roles;
            return View(roles);
        }
        [HttpGet]
        public async Task<IActionResult> EditUserInRole(string RoleId)
        {
            ViewBag.RoleId = RoleId;
            var role = await _roleManager.FindByIdAsync(RoleId);
            if (role == null)
            {
                return View("NotFound");
            }
            var ListUserRole = new List<UserRoleViewModel>();
            foreach (var user in _userManager.Users)
            {
                var UserRoleViewModel = new UserRoleViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName
                };

                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    UserRoleViewModel.IsSelected = true;
                }
                else
                {
                    UserRoleViewModel.IsSelected = false;
                }
                UserRoleViewModel.RoleId = RoleId;
                ListUserRole.Add(UserRoleViewModel);
            }
            return View(ListUserRole);
        }
        [HttpPost]
        public async Task<ActionResult> EditUserInRole(List<UserRoleViewModel> ListUserRole, string RoleId)
        {
            var role = await _roleManager.FindByIdAsync(RoleId);
            if (role == null)
            {
                return View("NotFound");
            }
            for (int i = 0; i < ListUserRole.Count; i++)
            {
                var user  = await _userManager.FindByIdAsync(ListUserRole[i].UserId);
                IdentityResult result = null;
                if (ListUserRole[i].IsSelected && !(await _userManager.IsInRoleAsync(user, role.Name)))
                {
                    result = await _userManager.AddToRoleAsync(user, role.Name);
                }
                else
                if (!ListUserRole[i].IsSelected && await _userManager.IsInRoleAsync(user, role.Name))
                {
                    result = await _userManager.RemoveFromRoleAsync(user, role.Name);
                }
                else
                {
                    continue;
                }
           
                    
            }
            return View("ListOfRoles", _roleManager.Roles);
        }
    }
}