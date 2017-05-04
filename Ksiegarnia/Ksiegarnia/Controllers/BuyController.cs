using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ksiegarnia.Models;

namespace Ksiegarnia.Controllers
{
    public class BuyController : Controller
    {
        KsiegarniaEntities1 db = new KsiegarniaEntities1();
        // GET: Buy
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Szczegoly(int id)
        {
            var details = db.Egzemplarze.Where(p => p.id_ksiazka == id).FirstOrDefault();
            return View(details);
        }

        public ActionResult getKsiazkiByKategoria(int id)
        {
            var details = db.Ksiazki.Where(p => p.id_kategoria == id);
            return View(details.ToList());
        }
    }
}