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

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public class IndexModel : PageModel
        {
            public void OnGet()
            {
                ViewData["ActivePage"] = "Home";
            }
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
