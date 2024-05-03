using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Pito.Models;
using System.Diagnostics;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;

namespace Pito.Controllers
{
    public class LoginController : Controller
    {
        private readonly LoginContext _context;

        // Constructor that uses dependency injection to provide an instance of LoginContext
        public LoginController(LoginContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Forgot()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Login loginModel)
        {
            if (string.IsNullOrWhiteSpace(loginModel.Email) || string.IsNullOrWhiteSpace(loginModel.Username) || string.IsNullOrWhiteSpace(loginModel.Password))
            {
                ViewBag.ErrorMessage = "Empty Registration";
                return View();
            }

            var alreadyCreated = _context.Logged.Any(user => user.Username.ToUpper() == loginModel.Username.ToUpper() || user.Email == loginModel.Email);
            if (alreadyCreated)
            {
                ViewBag.ErrorMessage = "A User Already Exists";
                return View();
            }
            else
            {
                loginModel.Password = HashUtility.HashPassword(loginModel.Password); // Hash the password before saving
                _context.Logged.Add(loginModel);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
        }

        // All existing comments and methods remain unchanged
        /*
          
        -: Prepare for password reset function later...

        [HttpPost]
        public IActionResult Create(Login loginModel)
        {
            if (string.IsNullOrWhiteSpace(loginModel.Username) || string.IsNullOrWhiteSpace(loginModel.Password))
            {
                ViewBag.ErrorMEssage = "Empty Registration";
                //Error 504 Innebär att det är fel
                return View();
            }
            //När vi fyller i formuläret behöver vi någonstans att spara denna data och vi använder oss utav databasen vi skapat i MovieContext.
            using (LoginContext db = new LoginContext())
            {
                db.Logged.Add(loginModel);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
         */

        [HttpPost]
        public async Task<IActionResult> Index(Login loginModel, string returnUrl = "")
        {
            var recaptcha = Request.Form["g-recaptcha-response"];
            if (!await ValidateRecaptcha(recaptcha))
            {
                ViewBag.ErrorMessage = "Captcha validation failed";
                ViewData["ReturnUrl"] = returnUrl;
                return View();
            }

            if (string.IsNullOrWhiteSpace(loginModel.Username) || string.IsNullOrWhiteSpace(loginModel.Password))
            {
                ViewBag.ErrorMessage = "Wrong Credentials";
                ViewData["ReturnUrl"] = returnUrl;
                return View();
            }

            var user = _context.Logged.FirstOrDefault(u => u.Username.ToUpper() == loginModel.Username.ToUpper() || u.Email.ToUpper() == loginModel.Username.ToUpper());
            if (user != null && HashUtility.VerifyPassword(user.Password, loginModel.Password))
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username) // Ensure the username is always used for the claim
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                return !string.IsNullOrEmpty(returnUrl) ? Redirect(returnUrl) : RedirectToAction("Index", "Home");
            }

            ViewBag.ErrorMessage = "Invalid login attempt.";
            ViewData["ReturnUrl"] = returnUrl;
            return View(loginModel);
        }

        private async Task<bool> ValidateRecaptcha(string recaptchaResponse)
        {
            var client = new HttpClient();
            var response = await client.PostAsync($"https://www.google.com/recaptcha/api/siteverify?secret=6Lcb7KcpAAAAADpMwdhwhUzbgpEdvHp6lDbQ82FB&response={recaptchaResponse}", new StringContent(""));
            var jsonString = await response.Content.ReadAsStringAsync();
            dynamic jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonString);
            return jsonData.success;
        }


        /*
        public async Task<IActionResult> SignOutUser()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Login");
        }
        */
    }
}
