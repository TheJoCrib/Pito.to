
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pito.Models;
using System.Security.Claims;

namespace Pito.Controllers
{
    public class LoginController : Controller
    {

        public IActionResult Index()
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
            //När vi fyller i formuläret behöver vi någonstans att spara denna data och vi använder oss utav databasen vi skapat i MovieContext.
            using (LoginContext db = new LoginContext())
            {
                db.Logged.Add(loginModel);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Index(Login loginModel, string returnUrl = "")
        {
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
                ViewBag.ErrorMessage = "Inloggningen inte godkänd";
                ViewData["ReturnUrl"] = returnUrl;
                return View();
            }
        }

        private bool ValidateUser(Login loginModel)
        {
            //Helt enkelt aktiverar databasen endast vid using, vi hämtar data från databasen som vi har skapat ovanför och jämfört det istället med information från databasen istället för "hårdkodad" sträng, 
            //Lättaste sättet att göra en inloggning under.
            using (var db = new LoginContext())
            {
                var existCheck = db.Logged.Any(user =>
                user.Username == loginModel.Username.ToUpper() && user.Password == loginModel.Password);


                return existCheck;
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
