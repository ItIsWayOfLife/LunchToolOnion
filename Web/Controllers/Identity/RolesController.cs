using ApplicationCore.Identity;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Web.Models.Roles;

namespace Web.Controllers.Identity
{
    [Authorize(Roles = "admin")]
    public class RolesController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RolesController> _logger;

        public RolesController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager,
            ILogger<RolesController> logger)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            _logger.LogInformation($"{DateTime.Now.ToString()}: Processing request Roles/Index");

            var listRoles = _roleManager.Roles;
            var listRolesView = new List<RoleViewModel>();

            foreach (var listRole in listRoles)
            {
                listRolesView.Add(
                    new RoleViewModel()
                    {
                        Id = listRole.Id,
                        Name = listRole.Name
                    });
            }
            return View(listRolesView);
        }

        [HttpGet]
        public IActionResult Create()
        {
            _logger.LogInformation($"{DateTime.Now.ToString()}: Processing request Roles/Create");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                IdentityResult result = await _roleManager.CreateAsync(new IdentityRole(name));
                if (result.Succeeded)
                {
                    string currentUserId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                    _logger.LogInformation($"{DateTime.Now.ToString()}: User {currentUserId} created new role");

                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                        _logger.LogError($"{DateTime.Now.ToString()}: {error.Code} {error.Description}");
                    }
                }
            }
            return View(name);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                IdentityRole role = await _roleManager.FindByIdAsync(id);
                if (role != null)
                {
                    IdentityResult result = await _roleManager.DeleteAsync(role);

                    string currentUserId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                    _logger.LogInformation($"{DateTime.Now.ToString()}: User {currentUserId} deleted role {id}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{DateTime.Now.ToString()}: {ex.ToString()}");

                return BadRequest();
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult UsersListByRole()
        {
            _logger.LogInformation($"{DateTime.Now.ToString()}: Processing request Roles/UsersListByRole");

            ICollection<ApplicationUser> listUsers = _userManager.Users.ToList();
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<ApplicationUser, UserByRoleViewModel>()).CreateMapper();
            var users = mapper.Map<IEnumerable<ApplicationUser>, List<UserByRoleViewModel>>(listUsers);

            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string userId)
        {
            _logger.LogInformation($"{DateTime.Now.ToString()}: Processing request Roles/Edit");

            // получаем пользователя
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                // получем список ролей пользователя
                var userRoles = await _userManager.GetRolesAsync(user);
                var allRoles = _roleManager.Roles.ToList();
                ChangeRoleViewModel model = new ChangeRoleViewModel
                {
                    UserId = user.Id,
                    UserEmail = user.Email,
                    UserRoles = userRoles,
                    AllRoles = allRoles
                };
                return View(model);
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string userId, List<string> roles)
        {
            // получаем пользователя
            ApplicationUser user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                // получем список ролей пользователя
                var userRoles = await _userManager.GetRolesAsync(user);
                // получаем все роли
                var allRoles = _roleManager.Roles.ToList();
                // получаем список ролей, которые были добавлены
                var addedRoles = roles.Except(userRoles);
                // получаем роли, которые были удалены
                var removedRoles = userRoles.Except(roles);

                await _userManager.AddToRolesAsync(user, addedRoles);

                await _userManager.RemoveFromRolesAsync(user, removedRoles);

                string currentUserId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                _logger.LogInformation($"{DateTime.Now.ToString()}: User {currentUserId} changed role for user {userId}");

                return RedirectToAction("UsersListByRole");
            }

            return NotFound();
        }
    }
}
