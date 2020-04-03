using ApplicationCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Web.Models.Users;

namespace Web.Controllers.Identity
{
    [Authorize(Roles = "admin")]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<UsersController> _logger;
        private readonly IStringLocalizer<SharedResource> _sharedLocalizer;

        public UsersController(UserManager<ApplicationUser> userManager,
             IStringLocalizer<SharedResource> sharedLocalizer,
             ILogger<UsersController> logger)
        {
            _userManager = userManager;
            _logger = logger;
            _sharedLocalizer = sharedLocalizer;
        }

        [HttpGet]
        public ActionResult Index(string searchSelectionString, string name)
        {
            _logger.LogInformation($"{DateTime.Now.ToString()}: Processing request Users/Index");

            var listUsers = _userManager.Users;
            var listViewUsers = new List<UserViewModel>();

            foreach (var listUser in listUsers)
            {
                listViewUsers.Add(
                    new UserViewModel()
                    {
                        Id = listUser.Id,
                        Email = listUser.Email,
                        FLP = $"{listUser.Lastname} {listUser.Firstname} {listUser.Patronomic}"
                    });
            }

            List<string> searchSelection = new List<string>() { _sharedLocalizer["SearchBy"], _sharedLocalizer["Id"], _sharedLocalizer["Email"], _sharedLocalizer["LFP"]};

            if (name == null)
                name = "";

            if (searchSelectionString == searchSelection[1])
            {
                listViewUsers = listViewUsers.Where(e => e.Id.ToLower().Contains(name.ToLower())).ToList();
            }

            if (searchSelectionString == searchSelection[2])
            {
                listViewUsers = listViewUsers.Where(e => e.Email.ToLower().Contains(name.ToLower())).ToList();
            }

            if (searchSelectionString == searchSelection[3])
            {
                listViewUsers = listViewUsers.Where(e => e.FLP.ToLower().Contains(name.ToLower())).ToList();
            }

            return View(new UserFilterListViewModel() {
                ListUsers = new ListUserViewModel { Users = listViewUsers },
                SearchSelection = new SelectList(searchSelection),
                SeacrhString = name,
                SearchSelectionString = searchSelectionString });
        }

        [HttpGet]
        public IActionResult Create()
        {
            _logger.LogInformation($"{DateTime.Now.ToString()}: Processing request Users/Create");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = new ApplicationUser { Email = model.Email,
                    UserName = model.Email,
                    Firstname = model.Firstname,
                    Lastname = model.Lastname,
                    Patronomic = model.Patronomic };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    string currentUserId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                    _logger.LogInformation($"{DateTime.Now.ToString()}: User {currentUserId} created new user: {model.Email}");

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
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            _logger.LogInformation($"{DateTime.Now.ToString()}: Processing request Users/Edit");

            ApplicationUser user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            EditUserViewModel model = new EditUserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                Patronomic = user.Patronomic
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditUserViewModel model)
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
                    user.Patronomic = model.Patronomic;

                    var result = await _userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        string currentUserId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                        _logger.LogInformation($"{DateTime.Now.ToString()}: User {currentUserId} edited user: {model.Id}");

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
            }
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Delete(string id)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                IdentityResult result = await _userManager.DeleteAsync(user);

                string currentUserId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                _logger.LogInformation($"{DateTime.Now.ToString()}: User {currentUserId} deleted user: {id}");

            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> ChangePassword(string id)
        {
            _logger.LogInformation($"{DateTime.Now.ToString()}: Processing request Users/ChangePassword");

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            ChangePasswordViewModel model = new ChangePasswordViewModel { Id = user.Id, Email = user.Email };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(Models.Users.ChangePasswordViewModel model)
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
                            string currentUserId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                            _logger.LogInformation($"{DateTime.Now.ToString()}: User {currentUserId} changed password user: {model.Id}");

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
                    else
                    {
                        ModelState.AddModelError(string.Empty, _sharedLocalizer["UserNotFound"]);
                        _logger.LogWarning($"{DateTime.Now.ToString()}: User not found");
                    }
                }
            }
            catch (ApplicationCore.Exceptions.ValidationException ex)
            {
                ModelState.AddModelError(ex.Property, ex.Message);
                _logger.LogError($"{DateTime.Now.ToString()}: {ex.Property}, {ex.Message}");
            }

            return View(model);
        }
    }
}
