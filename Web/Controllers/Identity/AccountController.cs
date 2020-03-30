using ApplicationCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Web.Models.Account;

namespace Web.Controllers.Identity
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
            ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register()
        {
            _logger.LogInformation($"{DateTime.Now.ToString()}: Processing request Account/Register");
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
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

                // добавляем пользователя
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // установка куки
                    await _signInManager.SignInAsync(user, false);

                    _logger.LogInformation($"{DateTime.Now.ToString()}: New user {_userManager.Users.Where(p=>p.Email==model.Email).FirstOrDefault().Id} ({user.Email}) registered") ;

                    return RedirectToAction("Index", "Home");                
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                        _logger.LogError($"{DateTime.Now.ToString()}: {error.Code} +{error.Description}");
                    }
                }
            }
            return View(model);
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            _logger.LogInformation($"{DateTime.Now.ToString()}: Processing request Account/Login");
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result =
                    await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    _logger.LogInformation($"{DateTime.Now.ToString()}: User {_userManager.Users.Where(p=>p.Email==model.Email).FirstOrDefault().Id} login");
                    // проверяем, принадлежит ли URL приложению
                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Неправильный логин и (или) пароль");
                }
            }
            return View(model);
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            string currentUserId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            _logger.LogInformation($"{DateTime.Now.ToString()}: User {currentUserId} logout");

            // удаляем аутентификационные куки
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Profile()
        {
            _logger.LogInformation($"{DateTime.Now.ToString()}: Processing request Account/Profile");
            if (User.Identity.IsAuthenticated)
            {
                ApplicationUser user = null;

                string currentUserId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                user = _userManager.Users.FirstOrDefault(p => p.Id == currentUserId);

                ProfileViewModel userView = new ProfileViewModel()
                {
                    Firstname = user.Firstname,
                    Lastname = user.Lastname,
                    Patronymic = user.Patronomic,
                    Email = user.Email
                };

                return View(userView);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public IActionResult Edit()
        {
            _logger.LogInformation($"{DateTime.Now.ToString()}: Processing request Account/Edit");
            if (User.Identity.IsAuthenticated)
            {
                ApplicationUser user = null;

                string currentUserId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                user = _userManager.Users.ToList().FirstOrDefault(p => p.Id == currentUserId);

                ProfileViewModel userView = new ProfileViewModel()
                {
                    Id = user.Id,
                    Firstname = user.Firstname,
                    Lastname = user.Lastname,
                    Patronymic = user.Patronomic,
                    Email = user.Email
                };

                return View(userView);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ProfileViewModel model)
        {
            if (User.Identity.IsAuthenticated)
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
                            string currentUserId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                            _logger.LogInformation($"{DateTime.Now.ToString()}: User {currentUserId} edited profile");

                            return RedirectToAction("Profile");
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
                }
                return View(model);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            _logger.LogInformation($"{DateTime.Now.ToString()}: Processing request Account/ChangePassword");
            if (User.Identity.IsAuthenticated)
            {
                ApplicationUser user = null;

                string currentUserId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                user = _userManager.Users.ToList().FirstOrDefault(p => p.Id == currentUserId);

                if (user == null)
                {
                    return NotFound();
                }

                ChangePasswordViewModel model = new ChangePasswordViewModel { Id = user.Id, Email = user.Email };

                return View(model);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = await _userManager.FindByIdAsync(model.Id);
                if (user != null)
                {
                    IdentityResult result =
                        await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        string currentUserId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                        _logger.LogInformation($"{DateTime.Now.ToString()}: User {currentUserId} changed password");
                        return RedirectToAction("Profile");
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
                else
                {
                    ModelState.AddModelError(string.Empty, "Пользователь не найден");
                    _logger.LogWarning($"{DateTime.Now.ToString()}: User not found");
                }
            }
            return View(model);
        }
    }
}
