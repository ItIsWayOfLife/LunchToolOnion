using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Web.Models;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            _logger.LogInformation($"{DateTime.Now.ToString()}: Processing request Home/Index");
            return RedirectToAction("ListFavoriteProvider", "Provider");
        }

        public IActionResult Privacy()
        {
            _logger.LogInformation($"{DateTime.Now.ToString()}: Processing request Home/Privacy");
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()             
        {
            _logger.LogInformation($"{DateTime.Now.ToString()}: Processing request Home/Error");
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
