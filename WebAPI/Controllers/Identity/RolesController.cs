using ApplicationCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebAPI.Identity.Models;

namespace WebAPI.Controllers.Identity
{
    [Route("api/roles")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class RolesController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RolesController> _logger;
        private readonly IUserHelper _userHelper;

        public RolesController(RoleManager<IdentityRole> roleManager,
              UserManager<ApplicationUser> userManager,
              ILogger<RolesController> logger,
              IUserHelper userHelper)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _logger = logger;
            _userHelper = userHelper;
        }

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var allRoles = _roleManager.Roles.ToList();
                List<string> allRolesStr = new List<string>();
                foreach (var role in allRoles)
                {
                    allRolesStr.Add(role.Name);
                }

                string currentEmail = this.User.FindFirst(ClaimTypes.Name).Value;
                string userId = _userHelper.GetUserId(currentEmail);

                if (userId == null)
                {
                    return NotFound("User not found");
                }

                _logger.LogInformation($"[{DateTime.Now.ToString()}]:[roles/get]:[info:get all roles]:[[user:{userId}]");

                return new ObjectResult(allRolesStr);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{DateTime.Now.ToString()}]:[roles/get]:[error:{ex}]");

                return BadRequest();
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            try
            {
                // получаем пользователя
                var user = await _userManager.FindByIdAsync(id);
                if (user != null)
                {
                    // получем список ролей пользователя
                    var userRoles = await _userManager.GetRolesAsync(user);

                    string currentEmail = this.User.FindFirst(ClaimTypes.Name).Value;
                    string userId = _userHelper.GetUserId(currentEmail);

                    if (userId == null)
                    {
                        return NotFound("User not found");
                    }

                    _logger.LogInformation($"[{DateTime.Now.ToString()}]:[roles/get/{id}]:[info:get roles user {id}]:[[user:{userId}]");

                    return new ObjectResult(userRoles);
                }

                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{DateTime.Now.ToString()}]:[roles/get/{id}]:[error:{ex}]");

                return BadRequest();
            }
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] UserChangeRoles model)
        {
            try
            {
                // получаем пользователя
                ApplicationUser user = await _userManager.FindByIdAsync(model.Id);
                if (user != null)
                {
                    // получем список ролей пользователя
                    var userRoles = await _userManager.GetRolesAsync(user);

                    // получаем список ролей, которые были добавлены
                    var addedRoles = model.Roles.Except(userRoles);

                    // получаем роли, которые были удалены
                    var removedRoles = userRoles.Except(model.Roles);

                    await _userManager.AddToRolesAsync(user, addedRoles);

                    await _userManager.RemoveFromRolesAsync(user, removedRoles);

                    string currentEmail = this.User.FindFirst(ClaimTypes.Name).Value;
                    string userId = _userHelper.GetUserId(currentEmail);

                    if (userId == null)
                    {
                        return NotFound("User not found");
                    }

                    _logger.LogInformation($"[{DateTime.Now.ToString()}]:[roles/put]:[info:user {userId} changed role for user {user.Id}]:[[user:{userId}]");

                    return Ok(model);
                }

                return NotFound("User not found");
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{DateTime.Now.ToString()}]:[roles/put]:[error:{ex}]");

                return BadRequest();
            }
        }
    }
}