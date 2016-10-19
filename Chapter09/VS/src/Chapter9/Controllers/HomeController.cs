using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Chapter9.Models;

namespace Chapter9.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View("~/Views/Shared/Error.cshtml");
        }
        public IActionResult Snowman()
        {
            return View("~/Views/Home/Snowman.cshtml");
        }

        public ActionResult Map()
        {
            StateDbContext DC = new StateDbContext();
            var states = from s in DC.States select s;

            return View(states);
        }
    }
}
