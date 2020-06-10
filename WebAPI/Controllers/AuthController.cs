using ApplicationCore.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IJwtConfigurator _jwtConfigurator;

        public AuthController(SignInManager<ApplicationUser> signInManager,
            IJwtConfigurator jwtConfigurator)
        {
            _signInManager = signInManager;
            _jwtConfigurator = jwtConfigurator;
        }

        public async Task<bool> CheckLogin(LoginModel model)
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
    }
}
