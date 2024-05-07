using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Mar.Models;
using System.Diagnostics;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace Mar.Controllers
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
                loginModel.Password = HashUtility.HashPassword(loginModel.Password);
                _context.Logged.Add(loginModel);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
        }
        [Authorize]
        public IActionResult Edit()
        {
            string currentUsername = User.Identity.Name;
            var user = _context.Logged.FirstOrDefault(u => u.Username == currentUsername);
            if (user == null)
            {
                return NotFound("User not found.");
            }
            return View(user);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Login model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = _context.Logged.FirstOrDefault(u => u.Id == model.Id);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Check if the email or username is being updated to a new value that already exists in the database
            if (_context.Logged.Any(u => u.Id != model.Id && (u.Email.ToUpper() == model.Email.ToUpper() || u.Username.ToUpper() == model.Username.ToUpper())))
            {
                ModelState.AddModelError(string.Empty, "Account with the same username or email already exists.");
                return View(model);
            }

            bool usernameChanged = user.Username != model.Username;
            bool passwordChanged = !HashUtility.VerifyPassword(model.Password, user.Password);

            user.Email = model.Email;
            user.Username = model.Username;
            user.Password = HashUtility.HashPassword(model.Password);  // Hashha alltid lösen

            _context.Update(user);
            await _context.SaveChangesAsync();

            //Fixat nu
            if (usernameChanged || passwordChanged)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return RedirectToAction("Index", "Login", new { message = "Please sign in again with your new username or password." });
            }

            return RedirectToAction("Index", "Home");
        }


        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Delete()
        {
            string currentUsername = User.Identity.Name;
            var user = _context.Logged.FirstOrDefault(u => u.Username == currentUsername);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            _context.Logged.Remove(user);
            await _context.SaveChangesAsync();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Login");
        }


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
                    new Claim(ClaimTypes.Name, user.Username)
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
            var response = await client.PostAsync($"https://www.google.com/recaptcha/api/siteverify?secret=6LcZItUpAAAAAIYNF3zGsJBmBx7CNMYoaKBxla98&response={recaptchaResponse}", new StringContent(""));
            var jsonString = await response.Content.ReadAsStringAsync();
            dynamic jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonString);
            return jsonData.success;
        }

        public async Task<IActionResult> SignOutUser()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Login");
        }
    }
}
