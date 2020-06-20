using ApplicationCore.Identity;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public ReportController(IReportService reportService,
              IProviderService providerService,
               IWebHostEnvironment webHostEnvironment,
               UserManager<ApplicationUser> userManager)
        {
            _reportService = reportService;
            _providerService = providerService;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
        }

        [HttpGet("provider/{providerId}/{dateWith?}/{dateTo?}")]
        public IActionResult GetReportProvider(int providerId, DateTime? dateWith = null, DateTime? dateTo = null)
        {
            var provider = _providerService.GetProvider(providerId);

            if (provider == null)
            {
                return NotFound("Provider not found");
            }

            List<List<string>> reportList;
            string title = "";

            if (dateWith != null && dateTo == null)
            {
                reportList = _reportService.GetReportProvider(providerId, dateWith.Value);
                title = $"Отчёт по поставщику ({provider.Name}) за {dateWith.Value.ToString("dd.MM.yyyy")}";
            }
            else if (dateWith != null && dateTo != null)
            {
                reportList = _reportService.GetReportProvider(providerId, dateWith.Value, dateTo.Value);
                title = $"Отчёт по поставщику ({provider.Name}) с {dateWith.Value.ToString("dd.MM.yyyy")} \nпо {dateTo.Value.ToString("dd.MM.yyyy")}";
            }
            else
            {
                reportList = _reportService.GetReportProvider(providerId);
                title = $"Отчёт по поставщику ({provider.Name}) за всё время";
            }

            ReportPDF reportProvider = new ReportPDF(_webHostEnvironment);
            
            return File(reportProvider.Report(reportList, title), "application/pdf");
        }

        [HttpGet("providers/{dateWith?}/{dateTo?}")]
        public IActionResult GetReportProviders(DateTime? dateWith = null, DateTime? dateTo = null)
        {

            List<List<string>> reportProvidersDTOs;
            string title = "";

            if (dateWith != null && dateTo == null)
            {
                reportProvidersDTOs = _reportService.GetReportProviders(dateWith.Value);
                title = $"Отчёт по поставщикам за {dateWith.Value.ToString("dd.MM.yyyy")}";
            }
            else if (dateWith != null && dateTo != null)
            {
                reportProvidersDTOs = _reportService.GetReportProviders(dateWith.Value, dateTo.Value);
                title = $"Отчёт по поставщикам с {dateWith.Value.ToString("dd.MM.yyyy")} \nпо {dateTo.Value.ToString("dd.MM.yyyy")}";
            }
            else
            {
                reportProvidersDTOs = _reportService.GetReportProviders();
                title = "Отчёт по поставщикам за всё время";
            }

            ReportPDF reportProviders = new ReportPDF(_webHostEnvironment);

            return File(reportProviders.Report(reportProvidersDTOs, title), "application/pdf");
        }

        [HttpGet("user/{userId}/{dateWith?}/{dateTo?}")]
        public IActionResult GetReportUser(string userId, DateTime? dateWith = null, DateTime? dateTo = null)
        {
            var user = _userManager.Users.Where(p => p.Id == userId).FirstOrDefault();

            if (user == null)
            {
                return NotFound("user not found");
            }
            List<List<string>> reportUserDTOs = null;
            string title = "";

            if (dateWith != null && dateTo == null)
            {
                reportUserDTOs = _reportService.GetReportUser(userId, dateWith.Value);
                title = $"Отчёт пользователя ({user.Email}) за {dateWith.Value.ToString("dd.MM.yyyy")}";
            }
            else if (dateWith != null && dateTo != null)
            {
                reportUserDTOs = _reportService.GetReportUser(userId, dateWith.Value, dateTo.Value);
                title = $"Отчёт пользователя ({user.Email}) с {dateWith.Value.ToString("dd.MM.yyyy")} \nпо {dateTo.Value.ToString("dd.MM.yyyy")}";
            }
            else
            {
                reportUserDTOs = _reportService.GetReportUser(userId);
                title = $"Отчёт пользователя ({user.Email}) за всё время";
            }

            ReportPDF reportUser = new ReportPDF(_webHostEnvironment);

            return File(reportUser.Report(reportUserDTOs, title), "application/pdf");
        }

        [HttpGet("users/{dateWith?}/{dateTo?}")]
        public IActionResult GetReportUsers(DateTime? dateWith = null, DateTime? dateTo = null)
        {
            List<List<string>> reportUsersDTOs = null;
            string title = "";

            if (dateWith != null && dateTo == null)
            {
                reportUsersDTOs = _reportService.GetReportUsers(dateWith.Value);
                title = $"Отчёт по всем пользователям за {dateWith.Value.ToString("dd.MM.yyyy")}";
            }
            else if (dateWith != null && dateTo != null)
            {
                reportUsersDTOs = _reportService.GetReportUsers(dateWith.Value, dateTo.Value);
                title = $"Отчёт по всем пользователям с {dateWith.Value.ToString("dd.MM.yyyy")} \nпо {dateTo.Value.ToString("dd.MM.yyyy")}";
            }
            else
            {
                reportUsersDTOs = _reportService.GetReportUsers();
                title = "Отчёт по всем пользователям за всё время";
            }

            ReportPDF reportUsers = new ReportPDF(_webHostEnvironment);

            return File(reportUsers.Report(reportUsersDTOs, title), "application/pdf");
        }
    }
}
