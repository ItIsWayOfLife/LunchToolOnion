using ApplicationCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
        private readonly IUserHelper _userHelper;

        public AccountController(SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            IJwtConfigurator jwtConfigurator,
            IUserHelper userHelper)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _jwtConfigurator = jwtConfigurator;
            _userHelper = userHelper;
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
                    return BadRequest("Invalid client request");
                }

                if (CheckLogin(user).Result)
                {
                    var tokenString = _jwtConfigurator.GetToken(user.UserName);
                    return Ok(new { Token = tokenString });
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                //ModelState.AddModelError(ex.Property, ex.Message);
                //_logger.LogError($"{DateTime.Now.ToString()}: {ex.Property}, {ex.Message}");
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
                        return Ok(model);
                    }
                    else
                    {
                        return BadRequest(result);
                    }
                }
                else
                {
                    return BadRequest(ModelState);
                }
            }
            catch (Exception ex)
            {
                //ModelState.AddModelError(ex.Property, ex.Message);
                //_logger.LogError($"{DateTime.Now.ToString()}: {ex.Property}, {ex.Message}");
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

                        return new ObjectResult(model);
                    }
                    else
                    {
                        return BadRequest("User not found");
                    }
                }
                else
                {
                    return BadRequest("Not authenticated");
                }
            }
            catch (Exception ex)
            {
                //ModelState.AddModelError(ex.Property, ex.Message);
                //_logger.LogError($"{DateTime.Now.ToString()}: {ex.Property}, {ex.Message}");
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
                                return Ok(model);
                            }
                            else
                            {
                                //foreach (var error in result.Errors)
                                //{
                                //    //ModelState.AddModelError(string.Empty, error.Description);
                                //}
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
                //ModelState.AddModelError(ex.Property, ex.Message);
                //_logger.LogError($"{DateTime.Now.ToString()}: {ex.Property}, {ex.Message}");
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
                            return Ok(model);
                        }
                        else
                        {
                            //foreach (var error in result.Errors)
                            //{
                            //    ModelState.AddModelError(string.Empty, error.Description);
                            //    _logger.LogError($"{DateTime.Now.ToString()}: {error.Code} {error.Description}");
                            //}

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
                //ModelState.AddModelError(ex.Property, ex.Message);
                //_logger.LogError($"{DateTime.Now.ToString()}: {ex.Property}, {ex.Message}");
                return BadRequest(ex);
            }
        }

    }
}
