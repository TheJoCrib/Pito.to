using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Pito.Models;
using System.Diagnostics;

namespace Pito.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly LoginContext _context;

        public HomeController(ILogger<HomeController> logger, LoginContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Error(int statusCode)
        {
            if (statusCode == 404)
            {
                // You can pass additional data to the view if you want
                return View("Error");
            }
            // Handle other status codes if necessary

            return View("Error");
        }

        public IActionResult Index()
        {
            ViewData["ActiveTab"] = "Home";
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
