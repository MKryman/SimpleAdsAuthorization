using Homework_04_19.Data;
using Homework_04_19.Web.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Homework_04_19.Web.Controllers
{
    public class AccountController : Controller
    {
        private string _connectionString = @"Data Source = .\sqlexpress; Initial Catalog = simpleAdsAuth; Integrated Security = true;";

        [Authorize]
        public IActionResult MyAccount()
        {
            var repo = new UserRepository(_connectionString);
            var user = repo.GetUserByEmail(User.Identity.Name);

            List<Ad> myAds = repo.GetAdsForUser(user.Id);

            MyAccountViewModel vm = new()
            {
                MyAds = myAds
            };
            
            return View(vm);
        }

        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SignUp(User user, string password)
        {
            var repo = new UserRepository(_connectionString);
            repo.NewUser(user, password);
            return Redirect("/account/login");
        }

        public IActionResult Login()
        {
            if (TempData["Message"] != null)
            {
                ViewBag.Message = TempData["Message"];
            }
            return View();
        }

        [HttpPost]
        public IActionResult Login(string userEmail, string passwordHash)
        {
            var repo = new UserRepository(_connectionString);
            var user = repo.Login(userEmail, passwordHash);

            if(user == null)
            {
                TempData["Message"] = "Invalid Login";
                return Redirect("/account/login");
            }


            var claims = new List<Claim>
            {
                new Claim("user", userEmail)
            };

            HttpContext.SignInAsync(new ClaimsPrincipal(
                new ClaimsIdentity(claims, "Cookies", "user", "role")))
                .Wait();

            return Redirect("/home/newad");
        }

        public IActionResult Logout()
        {
            HttpContext.SignOutAsync().Wait();
            return Redirect("/home/index");
        }
    }
}
