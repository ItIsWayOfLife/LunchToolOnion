using ApplicationCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtConfigurator _jwtConfigurator;

        public AccountController(SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            IJwtConfigurator jwtConfigurator)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _jwtConfigurator = jwtConfigurator;
        }

        private async Task<bool> CheckLogin(LoginModel model)
        {
            var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, true, false);
            return result.Succeeded;
        }

        // GET api/values
        [HttpPost, Route("login")]
        public IActionResult Login([FromBody] LoginModel user)
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

        // GET api/values
        [HttpPost, Route("register")]
        public async Task<IActionResult> Register(RegisterModel model)
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
                    return Ok();
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

        [HttpGet, Route("profile"), Authorize]
        public IActionResult Profile()
        {
            if (User.Identity.IsAuthenticated)
            {
                ApplicationUser user = null;

                string currentEmail = this.User.FindFirst(ClaimTypes.Name).Value;
                user = _userManager.Users.FirstOrDefault(p => p.Email == currentEmail);

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
                return BadRequest();
            }
        }


    }
}
