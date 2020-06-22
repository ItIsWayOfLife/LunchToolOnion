using ApplicationCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebAPI.Identity.Models;

namespace WebAPI.Controllers.Identity
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtConfigurator _jwtConfigurator;
        private readonly ILogger<AccountController> _logger;

        public AccountController(SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            IJwtConfigurator jwtConfigurator,
             ILogger<AccountController> logger
            )
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _jwtConfigurator = jwtConfigurator;
            _logger = logger;
        }

        private async Task<bool> CheckLogin(LoginModel model)
        {
            var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, true, false);
            return result.Succeeded;
        }

        [HttpPost, Route("login")]
        public IActionResult Login([FromBody] LoginModel user)
        {
            try
            {
                if (user == null)
                {
                    _logger.LogError($"[{DateTime.Now.ToString()}]:[account/login]:[error:invalid client request]:[user:{user.UserName}]");

                    return BadRequest("Invalid client request");
                }

                if (CheckLogin(user).Result)
                {
                    var tokenString = _jwtConfigurator.GetToken(user.UserName);

                    _logger.LogInformation($"[{DateTime.Now.ToString()}]:[account/login]:[info:login]:[user:{user.UserName}]");

                    return Ok(new { Token = tokenString });
                }
                else
                {
                    _logger.LogError($"[{DateTime.Now.ToString()}]:[account/login]:[error:unauthorized]:[user:{user.UserName}]");

                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{DateTime.Now.ToString()}]:[account/login]:[error:{ex}]:[user:{user.UserName}]");

                return BadRequest();
            }
        }

        [HttpPost, Route("register")]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            try
            {
                if (model == null)
                {
                    _logger.LogError($"[{DateTime.Now.ToString()}]:[account/register]:[error:invalid client request]:[user:{model.Email}]");

                    return BadRequest("Invalid client request");
                }

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

                    // добавляем пользователя
                    var result = await _userManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        _logger.LogInformation($"[{DateTime.Now.ToString()}]:[account/register]:[info:register]:[user:{model.Email}]");

                        return Ok(model);
                    }
                    else
                    {
                        _logger.LogError($"[{DateTime.Now.ToString()}]:[account/register]:[error:bad register]:[user:{model.Email}]");

                        return BadRequest(result);
                    }
                }
                else
                {
                    _logger.LogError($"[{DateTime.Now.ToString()}]:[account/register]:[error:no valid]:[user:{model.Email}]");

                    return BadRequest(ModelState);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{DateTime.Now.ToString()}]:[account/register]:[error:{ex}]:[user:{model.Email}]");

                return BadRequest();
            }
        }

        [HttpGet, Route("profile"), Authorize]
        public IActionResult Profile()
        {
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    ApplicationUser user = null;

                    string currentEmail = this.User.FindFirst(ClaimTypes.Name).Value;
                    user = _userManager.Users.FirstOrDefault(p => p.Email == currentEmail);

                    if (user != null)
                    {
                        ProfileModel model = new ProfileModel()
                        {
                            Firstname = user.Firstname,
                            Lastname = user.Lastname,
                            Patronymic = user.Patronomic,
                            Email = user.Email
                        };

                        _logger.LogInformation($"[{DateTime.Now.ToString()}]:[account/profile]:[info:get profile]:[user:{user.Id}]");

                        return new ObjectResult(model);
                    }
                    else
                    {
                        _logger.LogError($"[{DateTime.Now.ToString()}]:[account/profile]:[error:user not found]:[user:{currentEmail}]");
   
                        return BadRequest("User not found");
                    }
                }
                else
                {
                    string currentEmail = this.User.FindFirst(ClaimTypes.Name).Value;
                    _logger.LogError($"[{DateTime.Now.ToString()}]:[account/profile]:[error:not authenticated]:[user:{currentEmail}]");

                    return BadRequest("Not authenticated");
                }
            }
            catch (Exception ex)
            {
                string currentEmail = this.User.FindFirst(ClaimTypes.Name).Value;
                _logger.LogError($"[{DateTime.Now.ToString()}]:[account/profile]:[error:{ex}]:[user:{currentEmail}]");

                return BadRequest(ex);
            }
        }

        [HttpPost, Route("editprofile"), Authorize]
        [HttpPost]
        public async Task<IActionResult> Edit(ProfileModel model)
        {
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    if (ModelState.IsValid)
                    {
                        ApplicationUser user = null;

                        string currentEmail = this.User.FindFirst(ClaimTypes.Name).Value;
                        user = _userManager.Users.FirstOrDefault(p => p.Email == currentEmail);

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
                                _logger.LogInformation($"[{DateTime.Now.ToString()}]:[account/editprofile]:[info:edit profile]:[user:{user.Id}]");

                                return Ok(model);
                            }
                            else
                            {
                                foreach (var error in result.Errors)
                                {
                                    _logger.LogError($"[{DateTime.Now.ToString()}]:[account/editprofile]:[error:{error.Code}]:[user:{currentEmail}]");
                                }
                                return BadRequest(result.Errors);
                            }
                        }
                    }
                    return BadRequest("Model is not valid");
                }
                else
                {
                    return BadRequest(ModelState);
                }
            }
            catch (Exception ex)
            {
                string currentEmail = this.User.FindFirst(ClaimTypes.Name).Value;
                _logger.LogError($"[{DateTime.Now.ToString()}]:[account/editprofile]:[error:{ex}]:[user:{currentEmail}]");

                return BadRequest(ex);
            }
        }

        [HttpPost, Route("changepassword"), Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ApplicationUser user = null;

                    string currentEmail = this.User.FindFirst(ClaimTypes.Name).Value;
                    user = _userManager.Users.FirstOrDefault(p => p.Email == currentEmail);

                    if (user != null)
                    {
                        IdentityResult result =
                            await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                        if (result.Succeeded)
                        {
                            _logger.LogInformation($"[{DateTime.Now.ToString()}]:[account/changepassword]:[info:change password]:[user:{user.Id}]");

                            return Ok(model);
                        }
                        else
                        {
                            foreach (var error in result.Errors)
                            {
                              _logger.LogError($"[{DateTime.Now.ToString()}]:[account/changepassword]:[error:{error.Code}:{error.Description}]:[user:{currentEmail}]");
                            }

                            return BadRequest(result.Errors);
                        }
                    }
                    else
                    {
                        return BadRequest("User not found");
                    }
                }
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                string currentEmail = this.User.FindFirst(ClaimTypes.Name).Value;
                _logger.LogError($"[{DateTime.Now.ToString()}]:[account/changepassword]:[error:{ex}]:[user:{currentEmail}]");

                return BadRequest(ex);
            }
        }

    }
}
