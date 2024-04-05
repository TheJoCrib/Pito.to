using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Pito.Controllers
{
    public class CreditController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public class IndexModel : PageModel
        {
            public void OnGet()
            {
                ViewData["ActivePage"] = "Credit";
            }
        }
    }
}
