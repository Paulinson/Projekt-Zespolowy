using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ksiegarnia.Models;

namespace Ksiegarnia.Controllers
{
    public class HomeController : Controller
    {
        KsiegarniaEntities1 db = new KsiegarniaEntities1();
        public ActionResult Index()
        {
            var details = db.Ksiazki.ToList().Take(3);
            return View(details);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}