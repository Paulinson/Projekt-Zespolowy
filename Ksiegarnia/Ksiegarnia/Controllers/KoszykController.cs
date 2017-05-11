using Ksiegarnia.Infrastrcture;
using Ksiegarnia.Models;
using Ksiegarnia.ViewModels;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ksiegarnia.Controllers
{
    public class KoszykController : Controller
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private KoszykManager koszykManger;
        private KsiegarniaEntities1 db;
        private ISessionManager sessionManager { get; set; }

        public KoszykController(KsiegarniaEntities1 db, ISessionManager sessionManager)
        {
            this.db = db;
            this.sessionManager = sessionManager;
            koszykManger = new KoszykManager(sessionManager, db);
        }
        // GET: Koszyk
        public ActionResult Index()
        {
            var name = User.Identity.Name;
            logger.Info("Stron koszyk | " + name);
            var pozycjeKoszyka_ = koszykManger.pobierzKoszyk();
            var cenaCalkowita_ = koszykManger.pobierzWartoscKoszyka();

            KoszykViewModel koszykVM = new KoszykViewModel()
            {
                pozycjeKoszyka = pozycjeKoszyka_,
                cenaCalkowita = cenaCalkowita_
            };

            return View(koszykVM);
        }

        public ActionResult dodajDoKoszyka(int id)
        {
            koszykManger.dodajDoKoszyka(id);
            return RedirectToAction("Index");
        }

        public ActionResult usunZKoszyka(int id)
        {

            return View(); 
        }
    }
}