﻿using ApplicationCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public RolesController(RoleManager<IdentityRole> roleManager,
              UserManager<ApplicationUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
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
                return new ObjectResult(allRolesStr);
            }
            catch (Exception ex)
            {
                //ModelState.AddModelError(ex.Property, ex.Message);
                //_logger.LogError($"{DateTime.Now.ToString()}: {ex.Property}, {ex.Message}");
                return BadRequest(ex);
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

                    return new ObjectResult(userRoles);
                }

                return NotFound();
            }
            catch (Exception ex)
            {
                //ModelState.AddModelError(ex.Property, ex.Message);
                //_logger.LogError($"{DateTime.Now.ToString()}: {ex.Property}, {ex.Message}");
                return BadRequest(ex);
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

                    //string currentUserId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                    //_logger.LogInformation($"{DateTime.Now.ToString()}: User {currentUserId} changed role for user {userId}");

                    return Ok(model);
                }

                return NotFound("User not found");
            }
            catch (Exception ex)
            {
                //ModelState.AddModelError(ex.Property, ex.Message);
                //_logger.LogError($"{DateTime.Now.ToString()}: {ex.Property}, {ex.Message}");
                return BadRequest(ex);
            }
        }
    }
}
