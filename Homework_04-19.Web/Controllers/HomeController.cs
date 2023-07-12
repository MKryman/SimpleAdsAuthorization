using Homework_04_19.Data;
using Homework_04_19.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Homework_04_19.Web.Controllers
{
    public class HomeController : Controller
    {
        private string _connectionString = @"Data Source = .\sqlexpress; Initial Catalog = simpleAdsAuth; Integrated Security = true;";
        public IActionResult Index()
        {
            var repo = new UserRepository(_connectionString);
            HomePageviewModel vm = new()
            {
                Ads = repo.GetAds(),
                Repo = repo
            };

            return View(vm);
        }

        [Authorize]
        public IActionResult NewAd()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Redirect("/account/login");
            }
            return View();
        }

        [HttpPost]
        public IActionResult NewAd(Ad ad, string email)
        {
            var repo = new UserRepository(_connectionString);
            ad.UserId = repo.GetUserByEmail(email).Id;
            repo.InsertAd(ad);
            return RedirectToAction("index");
        }
    }
}