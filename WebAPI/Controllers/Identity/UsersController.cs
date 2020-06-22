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
    [Route("api/users")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<UsersController> _logger;
        private readonly IUserHelper _userHelper;

        public UsersController(UserManager<ApplicationUser> userManager,
            ILogger<UsersController> logger,
              IUserHelper userHelper)
        {
            _userManager = userManager;
            _logger = logger;
            _userHelper = userHelper;
        }

        [HttpGet]
        public ActionResult Get()
        {
            try
            {
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
            catch (Exception ex)
            {
                _logger.LogError($"[{DateTime.Now.ToString()}]:[users/get]:[error:{ex}]");

                return BadRequest(ex);
            }
        }

        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError($"[{DateTime.Now.ToString()}]:[users/get/{id}]:[error:{ex}]");

                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post(UserModel model)
        {
            try
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
                        string currentEmail = this.User.FindFirst(ClaimTypes.Name).Value;
                        string userId = _userHelper.GetUserId(currentEmail);

                        if (userId == null)
                        {
                            return NotFound("User not found");
                        }

                        _logger.LogInformation($"[{DateTime.Now.ToString()}]:[users/post]:[info:add user {model.Email}]:[[user:{userId}]");

                        return Ok(model);
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                         {
                            _logger.LogError($"[{DateTime.Now.ToString()}]:[users/post]:[error:{error.Code}|{error.Description}]");
                        }
                        return BadRequest(result);
                    }
                }
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{DateTime.Now.ToString()}]:[users/post]:[error:{ex}]");

                return BadRequest();
            }
        }

        [HttpPut]
        public async Task<IActionResult> Put(UserModel model)
        {
            try
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
                            string currentEmail = this.User.FindFirst(ClaimTypes.Name).Value;
                            string userId = _userHelper.GetUserId(currentEmail);

                            if (userId == null)
                            {
                                return NotFound("User not found");
                            }

                            _logger.LogInformation($"[{DateTime.Now.ToString()}]:[users/put]:[info:edit user {model.Email}]:[[user:{userId}]");

                            return Ok(model);
                        }
                        else
                        {
                            foreach (var error in result.Errors)
                            {
                                _logger.LogError($"[{DateTime.Now.ToString()}]:[users/put]:[error:{error.Code}|{error.Description}]");
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
            catch (Exception ex)
            {
                _logger.LogError($"[{DateTime.Now.ToString()}]:[users/put]:[error:{ex}]");

                return BadRequest();
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            try
            {
                ApplicationUser user = await _userManager.FindByIdAsync(id);

                if (user == null)
                {
                    return NotFound();
                }

                IdentityResult result = await _userManager.DeleteAsync(user);

                if (result.Succeeded)
                {
                    string currentEmail = this.User.FindFirst(ClaimTypes.Name).Value;
                    string userId = _userHelper.GetUserId(currentEmail);

                    if (userId == null)
                    {
                        return NotFound("User not found");
                    }

                    _logger.LogInformation($"[{DateTime.Now.ToString()}]:[users/delete/{id}]:[info:delete user {id}]:[[user:{userId}]");


                    return Ok(user);
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        _logger.LogError($"[{DateTime.Now.ToString()}]:[users/delete/{id}]:[error:{error.Code}|{error.Description}]");
                    }
                    return BadRequest(result.Errors);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{DateTime.Now.ToString()}]:[users/delete/{id}]:[error:{ex}]");

                return BadRequest();
            }
        }

        [HttpPost, Route("changepassword")]
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
                            string currentEmail = this.User.FindFirst(ClaimTypes.Name).Value;
                            string userId = _userHelper.GetUserId(currentEmail);

                            if (userId == null)
                            {
                                return NotFound("User not found");
                            }

                            _logger.LogInformation($"[{DateTime.Now.ToString()}]:[users/changepassword]:[info:change password user {model.Id}]:[[user:{userId}]");

                            return Ok(model);
                        }
                        else
                        {
                            foreach (var error in result.Errors)
                            {
                                _logger.LogError($"[{DateTime.Now.ToString()}]:[users/changepassword]:[error:{error.Code}|{error.Description}]");
                            }
                            return BadRequest(result.Errors);
                        }
                    }
                    else
                    {
                        return BadRequest("User not found");
                    }
                }
                else
                {
                    return BadRequest(ModelState);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{DateTime.Now.ToString()}]:[users/changepassword]:[error:{ex}]");

                return BadRequest();
            }
        }
    }

}