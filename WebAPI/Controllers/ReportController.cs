using ApplicationCore.Identity;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using WebAPI.Controllers.Identity;
using WebAPI.Reports;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;
        private readonly IProviderService _providerService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ReportController> _logger;
        private readonly IUserHelper _userHelper;
        public ReportController(IReportService reportService,
              IProviderService providerService,
               IWebHostEnvironment webHostEnvironment,
               UserManager<ApplicationUser> userManager,
               ILogger<ReportController> logger,
                IUserHelper userHelper)
        {
            _reportService = reportService;
            _providerService = providerService;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
            _logger = logger;
            _userHelper = userHelper;
        }

        [HttpGet("provider/{providerId}/{dateWith?}/{dateTo?}")]
        public IActionResult GetReportProvider(int providerId, DateTime? dateWith = null, DateTime? dateTo = null)
        {
            try
            {
                var provider = _providerService.GetProvider(providerId);

                if (provider == null)
                {
                    return NotFound("Provider not found");
                }

                string currentEmail = this.User.FindFirst(ClaimTypes.Name).Value;
                string userId = _userHelper.GetUserId(currentEmail);

                if (userId == null)
                {
                    return NotFound("User not found");
                }

                List<List<string>> reportList;
                string title = "";

                if (dateWith != null && dateTo == null)
                {
                    reportList = _reportService.GetReportProvider(providerId, dateWith.Value);
                    title = $"Отчёт по поставщику ({provider.Name}) за {dateWith.Value.ToString("dd.MM.yyyy")}";

                    _logger.LogInformation($"[{DateTime.Now.ToString()}]:[report/provider/{providerId}/{dateWith.Value.Date.ToShortDateString()}]:[info:create report by provider {providerId} for {dateWith.Value.Date.ToShortDateString()}]:[user:{userId}]");
                }
                else if (dateWith != null && dateTo != null)
                {
                    reportList = _reportService.GetReportProvider(providerId, dateWith.Value, dateTo.Value);
                    title = $"Отчёт по поставщику ({provider.Name}) с {dateWith.Value.ToString("dd.MM.yyyy")} \nпо {dateTo.Value.ToString("dd.MM.yyyy")}";

                    _logger.LogInformation($"[{DateTime.Now.ToString()}]:[report/provider/{providerId}/{dateWith.Value.Date.ToShortDateString()}/{dateTo.Value.Date.ToShortDateString()}]:[info:create report by provider {providerId} with {dateWith.Value.Date.ToShortDateString()} to {dateTo.Value.Date.ToShortDateString()}]:[user:{userId}]");
                }
                else
                {
                    reportList = _reportService.GetReportProvider(providerId);
                    title = $"Отчёт по поставщику ({provider.Name}) за всё время";

                    _logger.LogInformation($"[{DateTime.Now.ToString()}]:[report/provider/{providerId}]:[info:create report by provider {providerId}]:[user:{userId}]");
                }

                ReportPDF reportProvider = new ReportPDF(_webHostEnvironment);

                return File(reportProvider.Report(reportList, title), "application/pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{DateTime.Now.ToString()}]:[provider/{providerId}/{dateWith}/{dateTo}]:[error:{ex}]");

                return BadRequest();
            }
        }

        [HttpGet("providers/{dateWith?}/{dateTo?}")]
        public IActionResult GetReportProviders(DateTime? dateWith = null, DateTime? dateTo = null)
        {
            try
            {
                string currentEmail = this.User.FindFirst(ClaimTypes.Name).Value;
                string userId = _userHelper.GetUserId(currentEmail);

                if (userId == null)
                {
                    return NotFound("User not found");
                }

                List<List<string>> reportProvidersDTOs;
                string title = "";

                if (dateWith != null && dateTo == null)
                {
                    reportProvidersDTOs = _reportService.GetReportProviders(dateWith.Value);
                    title = $"Отчёт по поставщикам за {dateWith.Value.ToString("dd.MM.yyyy")}";

                    _logger.LogInformation($"[{DateTime.Now.ToString()}]:[report/providers/{dateWith.Value.Date.ToShortDateString()}]:[info:create report by all providers for {dateWith.Value.Date.ToShortDateString()}]:[user:{userId}]");

                }
                else if (dateWith != null && dateTo != null)
                {
                    reportProvidersDTOs = _reportService.GetReportProviders(dateWith.Value, dateTo.Value);
                    title = $"Отчёт по поставщикам с {dateWith.Value.ToString("dd.MM.yyyy")} \nпо {dateTo.Value.ToString("dd.MM.yyyy")}";

                    _logger.LogInformation($"[{DateTime.Now.ToString()}]:[report/providers/{dateWith.Value.Date.ToShortDateString()}/{dateTo.Value.Date.ToShortDateString()}]:[info:create report by all providers with {dateWith.Value.Date.ToShortDateString()} to {dateTo.Value.Date.ToShortDateString()}]:[user:{userId}]");
                }
                else
                {
                    reportProvidersDTOs = _reportService.GetReportProviders();
                    title = "Отчёт по поставщикам за всё время";

                    _logger.LogInformation($"[{DateTime.Now.ToString()}]:[report/providers]:[info:create report by all providers]:[user:{userId}]");

                }

                ReportPDF reportProviders = new ReportPDF(_webHostEnvironment);

                return File(reportProviders.Report(reportProvidersDTOs, title), "application/pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{DateTime.Now.ToString()}]:[providers/{dateWith}/{dateTo}]:[error:{ex}]");

                return BadRequest();
            }
        }

        [HttpGet("user/{userId}/{dateWith?}/{dateTo?}")]
        public IActionResult GetReportUser(string userId, DateTime? dateWith = null, DateTime? dateTo = null)
        {
            try
            {
                var user = _userManager.Users.Where(p => p.Id == userId).FirstOrDefault();

                if (user == null)
                {
                    return NotFound("User not found");
                }

                string currentEmail = this.User.FindFirst(ClaimTypes.Name).Value;
                string myUserId = _userHelper.GetUserId(currentEmail);

                if (myUserId == null)
                {
                    return NotFound("User not found");
                }

                List<List<string>> reportUserDTOs = null;
                string title = "";

                if (dateWith != null && dateTo == null)
                {
                    reportUserDTOs = _reportService.GetReportUser(userId, dateWith.Value);
                    title = $"Отчёт пользователя ({user.Email}) за {dateWith.Value.ToString("dd.MM.yyyy")}";

                    _logger.LogInformation($"[{DateTime.Now.ToString()}]:[report/user/{userId}/{dateWith.Value.Date.ToShortDateString()}]:[info:create report by user {userId} for {dateWith.Value.Date.ToShortDateString()}]:[user:{myUserId}]");

                }
                else if (dateWith != null && dateTo != null)
                {
                    reportUserDTOs = _reportService.GetReportUser(userId, dateWith.Value, dateTo.Value);
                    title = $"Отчёт пользователя ({user.Email}) с {dateWith.Value.ToString("dd.MM.yyyy")} \nпо {dateTo.Value.ToString("dd.MM.yyyy")}";

                    _logger.LogInformation($"[{DateTime.Now.ToString()}]:[report/user/{userId}/{dateWith.Value.Date.ToShortDateString()}/{dateTo.Value.Date.ToShortDateString()}]:" +
                        $"[info:create report by user {userId} with {dateWith.Value.Date.ToShortDateString()} to {dateTo.Value.Date.ToShortDateString()}]:[user:{myUserId}]");
                }
                else
                {
                    reportUserDTOs = _reportService.GetReportUser(userId);
                    title = $"Отчёт пользователя ({user.Email}) за всё время";

                    _logger.LogInformation($"[{DateTime.Now.ToString()}]:[report/user/{userId}]:[info:create report by user {userId}]:[user:{myUserId}]");
                }

                ReportPDF reportUser = new ReportPDF(_webHostEnvironment);

                return File(reportUser.Report(reportUserDTOs, title), "application/pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{DateTime.Now.ToString()}]:user/{userId}/{dateWith}/{dateTo}]:[error:{ex}]");

                return BadRequest();
            }
        }

        [HttpGet("users/{dateWith?}/{dateTo?}")]
        public IActionResult GetReportUsers(DateTime? dateWith = null, DateTime? dateTo = null)
        {
            try
            {
                string currentEmail = this.User.FindFirst(ClaimTypes.Name).Value;
                string myUserId = _userHelper.GetUserId(currentEmail);

                if (myUserId == null)
                {
                    return NotFound("User not found");
                }

                List<List<string>> reportUsersDTOs = null;
                string title = "";

                if (dateWith != null && dateTo == null)
                {
                    reportUsersDTOs = _reportService.GetReportUsers(dateWith.Value);
                    title = $"Отчёт по всем пользователям за {dateWith.Value.ToString("dd.MM.yyyy")}";

                    _logger.LogInformation($"[{DateTime.Now.ToString()}]:[report/users/{dateWith.Value.Date.ToShortDateString()}]:" +
                        $"[info:create report by all users for {dateWith.Value.Date.ToShortDateString()}]:[user:{myUserId}]");

                }
                else if (dateWith != null && dateTo != null)
                {
                    reportUsersDTOs = _reportService.GetReportUsers(dateWith.Value, dateTo.Value);
                    title = $"Отчёт по всем пользователям с {dateWith.Value.ToString("dd.MM.yyyy")} \nпо {dateTo.Value.ToString("dd.MM.yyyy")}";

                    _logger.LogInformation($"[{DateTime.Now.ToString()}]:[report/users/{dateWith.Value.Date.ToShortDateString()}/{dateTo.Value.Date.ToShortDateString()}]:" +
                         $"[info:create report by all users with {dateWith.Value.Date.ToShortDateString()} to {dateTo.Value.Date.ToShortDateString()}]:[user:{myUserId}]");

                }
                else
                {
                    reportUsersDTOs = _reportService.GetReportUsers();
                    title = "Отчёт по всем пользователям за всё время";

                    _logger.LogInformation($"[{DateTime.Now.ToString()}]:[report/users]:[info:create report by all users]:[user:{myUserId}]");
                }

                ReportPDF reportUsers = new ReportPDF(_webHostEnvironment);

                return File(reportUsers.Report(reportUsersDTOs, title), "application/pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{DateTime.Now.ToString()}]:users/{dateWith}/{dateTo}]:[error:{ex}]");

                return BadRequest();
            }
        }
    }
}
