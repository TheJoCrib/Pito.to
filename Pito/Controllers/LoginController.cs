
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pito.Models;
using System.Diagnostics;
using System.Security.Claims;

namespace Pito.Controllers
{
    public class LoginController : Controller
    {

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Forgot()
        {
            return View();
        }

        //För att skicka client-side data till en server(vår databas), du hämtar datan.
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }



        //För att skicka client-side data till en server (vår databas), du skickar datan.
        [HttpPost]
        public IActionResult Create(Login loginModel)
        {
            if (string.IsNullOrWhiteSpace(loginModel.Email) || string.IsNullOrWhiteSpace(loginModel.Username) || string.IsNullOrWhiteSpace(loginModel.Password))
            {
                ViewBag.ErrorMEssage = "Empty Registration";

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
                ViewBag.ErrorMEssage = "Wrong Credentials";
                //Error 504 Innebär att det är fel
                ViewData["ReturnUrl"] = returnUrl;
                return View();
            }
            //Checkar Validate Method om vår databas har liknande som loginModel
            bool validUser = ValidateUser(loginModel);

            if (validUser == true)
            {
                var user = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                user.AddClaim(new Claim(ClaimTypes.Name, loginModel.Username));

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(user));

                if (returnUrl != "")
                {
                    return Redirect(returnUrl);
                }
                else
                    return RedirectToAction("Index", "Home");
            }
            else
            {

                ViewBag.ErrorMessage = "Wrong Credentials";
                ViewData["ReturnUrl"] = returnUrl;
                return View();
            }
        }

        private async Task<bool> ValidateRecaptcha(string recaptchaResponse)
        {
            var client = new HttpClient();
            var response = await client.PostAsync($"https://www.google.com/recaptcha/api/siteverify?secret=6Lcb7KcpAAAAADpMwdhwhUzbgpEdvHp6lDbQ82FB&response={recaptchaResponse}", new StringContent(""));
            var jsonString = await response.Content.ReadAsStringAsync();
            dynamic jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonString);
            return jsonData.success;
        }
        private bool ValidateUser(Login loginModel)
        {
            //Helt enkelt aktiverar databasen endast vid using, vi hämtar data från databasen som vi har skapat ovanför och jämfört det istället med information från databasen istället för "hårdkodad" sträng, 
            //Lättaste sättet att göra en inloggning under.

            using (var db = new LoginContext())
            {

                var existCheck1 = db.Logged.Any(user => (user.Username == loginModel.Username.ToUpper()) || (user.Email == loginModel.Username) && user.Password == loginModel.Password);



                return existCheck1;

            }

        }

        public async Task<IActionResult> SignOutUser()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "Login");
        }


    }
}
