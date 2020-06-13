﻿using ApplicationCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [Route("api/users")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
      
        public UsersController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
      
        [HttpGet]
        public ActionResult Get()
        {
            //_logger.LogInformation($"{DateTime.Now.ToString()}: Processing request Users/Index");

            var listUsers = _userManager.Users;
            var listViewUsers = new List<UserModel>();

            foreach (var listUser in listUsers)
            {
                listViewUsers.Add(
                    new UserModel()
                    {
                        Id = listUser.Id,
                        Email = listUser.Email,
                        Firstname = listUser.Firstname,
                        Lastname = listUser.Lastname,
                        Patronymic = listUser.Patronomic,
                        Password = null
                    });
            }

            return new ObjectResult(listViewUsers);
        }

        // GET api/users/5
        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            var user = _userManager.Users.FirstOrDefault(x => x.Id == id);
            if (user == null)
                return NotFound();

            UserModel userViewModel = new UserModel()
            {
                Id = user.Id,
                Email = user.Email,
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                Patronymic = user.Patronomic,
                Password = null
            };

            return new ObjectResult(userViewModel);
        }

        // POST api/users
        [HttpPost]
        public async Task<IActionResult> Post(UserModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = new ApplicationUser
                {
                    Email = model.Email,
                    UserName = model.Email,
                    Firstname = model.Firstname,
                    Lastname = model.Lastname,
                    Patronomic = model.Patronymic
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    //string currentUserId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                    //_logger.LogInformation($"{DateTime.Now.ToString()}: User {currentUserId} created new user: {model.Email}");

                    return Ok(model);
                }
                else
                {                   
                   foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                        //_logger.LogError($"{DateTime.Now.ToString()}: {error.Code} {error.Description}");
                    }
                    return BadRequest(result);
                }
            }
            return BadRequest("No valid");
        }

        // PUT api/users/
        [HttpPut]
        public async Task<IActionResult> Put(UserModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = await _userManager.FindByIdAsync(model.Id);
                if (user != null)
                {
                    user.Email = model.Email;
                    user.UserName = model.Email;
                    user.Firstname = model.Firstname;
                    user.Lastname = model.Lastname;
                    user.Patronomic = model.Patronymic;

                    var result = await _userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        //string currentUserId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                        //_logger.LogInformation($"{DateTime.Now.ToString()}: User {currentUserId} edited user: {model.Id}");

                        return Ok(model);
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                            //_logger.LogError($"{DateTime.Now.ToString()}: {error.Code} {error.Description}");
                        }
                    }
                }
                else
                {
                    return BadRequest("User not found");
                }
            }
            return BadRequest("No valid");
        }

        // DELETE api/users/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(id);

            if (user==null)
            {
                return NotFound();
            }

                IdentityResult result = await _userManager.DeleteAsync(user);


            if (result.Succeeded)
            {
                return Ok(user);
            }
            //string currentUserId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            //_logger.LogInformation($"{DateTime.Now.ToString()}: User {currentUserId} deleted user: {id}");
              return NotFound(); 
        }

        [HttpPost, Route("changePassword")]
        public async Task<IActionResult> ChangePassword(UserModelChangePasword model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await _userManager.FindByIdAsync(model.Id);
                    if (user != null)
                    {
                        IdentityResult result =
                            await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                        if (result.Succeeded)
                        {
                            //string currentUserId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                            //_logger.LogInformation($"{DateTime.Now.ToString()}: User {currentUserId} changed password user: {model.Id}");

                            return Ok(model);
                        }
                        else
                        {
                            foreach (var error in result.Errors)
                            {
                                //ModelState.AddModelError(string.Empty, error.Description);
                                //_logger.LogError($"{DateTime.Now.ToString()}: {error.Code} {error.Description}");

                                return BadRequest(error);
                            }
                        }
                    }
                    else
                    {
                        //ModelState.AddModelError(string.Empty, _sharedLocalizer["UserNotFound"]);
                        //_logger.LogWarning($"{DateTime.Now.ToString()}: User not found");
                        return BadRequest("User not found");
                    }
                }
                else
                {
                    return BadRequest("No valid");
                }
            }
            catch (ApplicationCore.Exceptions.ValidationException ex)
            {
                ModelState.AddModelError(ex.Property, ex.Message);
                //_logger.LogError($"{DateTime.Now.ToString()}: {ex.Property}, {ex.Message}");
                return BadRequest(ex);
            }

            return BadRequest(model);
        }

       
    }
}