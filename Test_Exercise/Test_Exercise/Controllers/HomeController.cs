using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Test_Exercise.Models;
using Test_Exercise.Data;

namespace Test_Exercise.Controllers
{
    public class HomeController : Controller
    {
        public static List<Massage> allMassages = new List<Massage>();
        public List<Massage> userMassages;
        public IActionResult Index()
        {
            if (Request.Cookies["UserID"] == null)
            {
                Random rnd = new Random();
                int userId = rnd.Next(10000, 99999);
                CookieOptions cookieOptions = new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(1)
                };
                Response.Cookies.Append("UserID", Convert.ToString(userId), cookieOptions);
            }
            return View();
        }
        [HttpPost]
        public IActionResult Index(Massage massage)
        {
            Random rnd = new Random();
            massage.Id = rnd.Next(100000, 999999);
            massage.UserID = Request.Cookies["UserID"];
            massage.Time = DateTime.Now;

            if (allMassages.Count > 19)
                allMassages.RemoveAt(0);

            userMassages = allMassages.Where(s => s.UserID == massage.UserID).ToList();
            if (userMassages.Count > 9)
            {
                var item = allMassages.FirstOrDefault(c => c.UserID == massage.UserID);
                allMassages.Remove(item);
            }

            allMassages.Add(massage);

            return RedirectToAction("UserMassages");
        }

        public IActionResult UserMassages()
        {
            userMassages = allMassages.Where(s => s.UserID == Request.Cookies["UserID"]).ToList();
            return View(userMassages);
        }

        public IActionResult AllMassages(SortState sortOrder = SortState.IdAsc)
        {
            ViewBag.IdSort = sortOrder == SortState.IdAsc ? SortState.IdDesc : SortState.IdAsc;
            ViewBag.TimeSort = sortOrder == SortState.TimeAsc ? SortState.TimeDesc : SortState.TimeAsc;

            var result = allMassages;
            result = sortOrder switch
            {
                SortState.IdDesc => allMassages.OrderByDescending(s => s.Id).ToList(),
                SortState.TimeAsc => allMassages.OrderBy(s => s.Time).ToList(),
                SortState.TimeDesc => allMassages.OrderByDescending(s => s.Time).ToList(),
                _ => allMassages.OrderBy(s => s.Id).ToList()
            };

            return View(result);
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}