using Ksiegarnia.Infrastrcture;
using Ksiegarnia.Models;
using Ksiegarnia.ViewModels;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

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
            logger.Info("Strona koszyk | " + name);
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

        public int pobierzIloscElementowKoszyka()
        {
            return koszykManger.pobierzIloscPozycjiKoszyka();
        }

        public ActionResult usunZKoszyka(int id)
        {
            int iloscPozycji = koszykManger.usunZKoszyka(id);
            int iloscPozycjiKoszyka = koszykManger.pobierzIloscPozycjiKoszyka();
            decimal wartoscKoszyka = koszykManger.pobierzWartoscKoszyka();

            var wynik = new UsunZKoszykaViewModel
            {
                idPozycjiUsuwanej = id,
                iloscPozycjiUsuwanej = iloscPozycji,
                koszykCenaCalkowita = wartoscKoszyka,
                koszykIloscPozycji = iloscPozycjiKoszyka
            };

            return Json(wynik);
        }

        //public async Task<ActionResult> zaplac()
        //{
        //    var name = User.Identity.Name;
        //    logger.Info("Strona koszyk | Zaplać | " + name);

        //    if (Request.IsAuthenticated)
        //    {
        //        var user = await userManager.FindByIdAsync(User.Identity.GetUserId());

        //        var zamowienie = new Zamowienia
        //        {
                    
        //        }
        //    }
        //}

        public ApplicationUserManager _userManager;
        public ApplicationUserManager userManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
    }
}