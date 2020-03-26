using ApplicationCore.DTO;
using ApplicationCore.Exceptions;
using ApplicationCore.Identity;
using ApplicationCore.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Web.Models.Report;
using Web.Reports;

namespace Web.Controllers
{
    [Authorize(Roles = "admin")]
    public class ReportController : Controller
    {
        private readonly IReportService _reportService;
        private readonly IProviderService _providerService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ReportController> _logger;

        public ReportController(IReportService reportService, IWebHostEnvironment webHostEnvironment,
            IProviderService providerService, UserManager<ApplicationUser> userManager,
            ILogger<ReportController> logger)
        {
            _userManager = userManager;
            _reportService = reportService;
            _webHostEnvironment = webHostEnvironment;
            _providerService = providerService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            _logger.LogInformation($"{DateTime.Now.ToString()}: Processing request Report/Index");
            return View();
        }

        [HttpPost]
        public IActionResult GetReportProvider(int? providerId, DateTime? dateWith = null, DateTime? dateTo = null)
        {
            _logger.LogInformation($"{DateTime.Now.ToString()}: Processing request Report/GetReportProvider");

            try
            {
                var provider = _providerService.GetProvider(providerId);

                if (provider == null)
                {
                    throw new ValidationException("Поставщик не найден", "");
                }
                
                IEnumerable<ReportProviderDTO> reportProviderDTOs;
                string title = "";

                if (dateWith != null && dateTo == null)
                {
                    reportProviderDTOs = _reportService.GetReportProvider(providerId, dateWith.Value);
                    title = $"Отчёт по поставщику ({provider.Name}) за {dateWith.Value.ToString("dd.MM.yyyy")}";
                }
                else if (dateWith != null && dateTo != null)
                {
                    reportProviderDTOs = _reportService.GetReportProvider(providerId, dateWith.Value, dateTo.Value);
                    title = $"Отчёт по поставщику ({provider.Name}) с {dateWith.Value.ToString("dd.MM.yyyy")} \n по {dateTo.Value.ToString("dd.MM.yyyy")}";
                }
                else
                {
                    reportProviderDTOs = _reportService.GetReportProvider(providerId);
                    title = $"Отчёт по поставщику ({provider.Name}) за всё время";
                }
                var mapper = new MapperConfiguration(cfg => cfg.CreateMap<ReportProviderDTO, ReportProviderViewModel>()).CreateMapper();
                var reportProviders = mapper.Map<IEnumerable<ReportProviderDTO>, List<ReportProviderViewModel>>(reportProviderDTOs);

                ReportProvider reportProvider = new ReportProvider(_webHostEnvironment);

                _logger.LogInformation($"{DateTime.Now.ToString()}: Created report by provider {providerId.ToString()}");

                return File(reportProvider.Report(reportProviders, provider.Path, provider.Name.ToUpper(), title, 4), "application/pdf");

            }
            catch (ValidationException ex)
            {
                ModelState.AddModelError(ex.Property, ex.Message);
                _logger.LogError($"{DateTime.Now.ToString()}: {ex.Property}, {ex.Message}");
            }

            return BadRequest("Некорректный запрос");
        }

        [HttpPost]
        public IActionResult GetReportProviders(DateTime? dateWith = null, DateTime? dateTo = null)
        {
            _logger.LogInformation($"{DateTime.Now.ToString()}: Processing request Report/GetReportProviders");

            try
            {
                IEnumerable<ReportProvidersDTO> reportProvidersDTOs;
                string title = "";

                if (dateWith != null && dateTo == null)
                {
                    reportProvidersDTOs = _reportService.GetReportProviders(dateWith.Value);
                    title = $"Отчёт по поставщикам за {dateWith.Value.ToString("dd.MM.yyyy")}";
                }
                else if (dateWith != null && dateTo != null)
                {
                    reportProvidersDTOs = _reportService.GetReportProviders(dateWith.Value, dateTo.Value);
                    title = $"Отчёт по поставщикам с {dateWith.Value.ToString("dd.MM.yyyy")} \n по {dateTo.Value.ToString("dd.MM.yyyy")}";
                }
                else
                {
                    reportProvidersDTOs = _reportService.GetReportProviders();
                    title = $"Отчёт по поставщикам за все время";
                }

                var mapper = new MapperConfiguration(cfg => cfg.CreateMap<ReportProvidersDTO, ReportProvidersViewModel>()).CreateMapper();
                var reportProvidersList = mapper.Map<IEnumerable<ReportProvidersDTO>, List<ReportProvidersViewModel>>(reportProvidersDTOs);

                ReportProviders reportProviders = new ReportProviders(_webHostEnvironment);

                _logger.LogInformation($"{DateTime.Now.ToString()}: Created report by providers");

                return File(reportProviders.Report(reportProvidersList, title, 5), "application/pdf");

            }
            catch (ValidationException ex)
            {
                ModelState.AddModelError(ex.Property, ex.Message);
                _logger.LogError($"{DateTime.Now.ToString()}: {ex.Property}, {ex.Message}");
            }

            return BadRequest("Некорректный запрос");
        }

        [HttpPost]
        public IActionResult GetReportUser(string userId, DateTime? dateWith = null, DateTime? dateTo = null)
        {
            _logger.LogInformation($"{DateTime.Now.ToString()}: Processing request Report/GetReportUser");

            try
            {
                var user = _userManager.Users.Where(p => p.Id == userId).FirstOrDefault();

                if (user == null)
                {
                    throw new ValidationException("Пользователей не найден", "");
                }
                IEnumerable<ReportUserDTO> reportUserDTOs = null;
                string title = "";

                if (dateWith != null && dateTo == null)
                {
                    reportUserDTOs = _reportService.GetReportUser(userId, dateWith.Value);
                    title = $"Отчёт по пользователю ({user.Email}) за {dateWith.Value.ToString("dd.MM.yyyy")}";
                }
                else if (dateWith != null && dateTo != null)
                {
                    reportUserDTOs = _reportService.GetReportUser(userId, dateWith.Value, dateTo.Value);
                    title = $"Отчёт по пользователю ({user.Email}) с {dateWith.Value.ToString("dd.MM.yyyy")} \n по {dateTo.Value.ToString("dd.MM.yyyy")}";
                }
                else
                {
                    reportUserDTOs = _reportService.GetReportUser(userId);
                    title = $"Отчёт по пользователю ({user.Id}) за всё время";
                }
                var mapper = new MapperConfiguration(cfg => cfg.CreateMap<ReportUserDTO, ReportUserViewModel>()).CreateMapper();
                var reportUsers = mapper.Map<IEnumerable<ReportUserDTO>, List<ReportUserViewModel>>(reportUserDTOs);

                ReportUser reportUser = new ReportUser(_webHostEnvironment);

                _logger.LogInformation($"{DateTime.Now.ToString()}: Created report by user {userId.ToString()}");

                return File(reportUser.Report(reportUsers, title, 4), "application/pdf");
            }
            catch (ValidationException ex)
            {
                ModelState.AddModelError(ex.Property, ex.Message);
                _logger.LogError($"{DateTime.Now.ToString()}: {ex.Property}, {ex.Message}");
            }

            return BadRequest("Некорректный запрос");
        }

        [HttpPost]
        public IActionResult GetReportUsers(DateTime? dateWith = null, DateTime? dateTo = null)
        {
            _logger.LogInformation($"{DateTime.Now.ToString()}: Processing request Report/GetReportUsers");

            try
            {
                IEnumerable<ReportUsersDTO> reportUsersDTOs = null;
                string title = "";

                if (dateWith != null && dateTo == null)
                {
                    reportUsersDTOs = _reportService.GetReportUsers(dateWith.Value);
                    title = $"Отчёт по всем пользователям за {dateWith.Value.ToString("dd.MM.yyyy")}";
                }
                else if (dateWith != null && dateTo != null)
                {
                    reportUsersDTOs = _reportService.GetReportUsers(dateWith.Value, dateTo.Value);
                    title = $"Отчётпо всем пользователям с {dateWith.Value.ToString("dd.MM.yyyy")} \n по {dateTo.Value.ToString("dd.MM.yyyy")}";
                }
                else
                {
                    reportUsersDTOs = _reportService.GetReportUsers();
                    title = $"Отчёт по по всем пользователям за всё время";
                }
                var mapper = new MapperConfiguration(cfg => cfg.CreateMap<ReportUsersDTO, ReportUsersViewModel>()).CreateMapper();
                var reportUsersList = mapper.Map<IEnumerable<ReportUsersDTO>, List<ReportUsersViewModel>>(reportUsersDTOs);

                ReportUsers reportUsers = new ReportUsers(_webHostEnvironment);

                _logger.LogInformation($"{DateTime.Now.ToString()}: Created report by users");

                return File(reportUsers.Report(reportUsersList, title, 6), "application/pdf");
            }
            catch (ValidationException ex)
            {
                ModelState.AddModelError(ex.Property, ex.Message);
                _logger.LogError($"{DateTime.Now.ToString()}: {ex.Property}, {ex.Message}");
            }

            return BadRequest("Некорректный запрос");
        }
    }
}
