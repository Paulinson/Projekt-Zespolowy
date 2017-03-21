using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Owin;
using Ksiegarnia.Models;
using System.Net.Mail;
using System.Net;

namespace Ksiegarnia.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        KsiegarniaEntities db = new KsiegarniaEntities();

        private ApplicationUserManager _userManager;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }

        public ApplicationUserManager UserManager {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult Register(Uzytkownicy usr)
        {
            if(ModelState.IsValid)
            {
                var details = db.Uzytkownicy.Where(p => p.email == usr.email).FirstOrDefault();
                if (details == null)
                {
                    db.Uzytkownicy.Add(usr);
                    db.SaveChanges();
                    SendActivationEmail(usr);
                    return RedirectToAction("Welcome");
                }
                else
                {
                    ViewBag.message = "Error";
                }
            }
            return View();
        }

        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult Login(Uzytkownicy usr)
        {
            #region modelstate remove
            ModelState.Remove("imie");
            ModelState.Remove("nazwisko");
            ModelState.Remove("wiek");
            ModelState.Remove("nr_telefonu");
            ModelState.Remove("aktywny");
            ModelState.Remove("ulica");
            ModelState.Remove("miasto");
            ModelState.Remove("nr_domu");
            ModelState.Remove("nr_mieszkania");
            ModelState.Remove("wojewodztwo");
            ModelState.Remove("kod_pocztowy");
            ModelState.Remove("haslo_potw");
            #endregion

            if (ModelState.IsValid)
            {
                var details = db.Uzytkownicy.Where(a => a.email.Equals(usr.email) && a.haslo.Equals(usr.haslo) && a.aktywny == 1).FirstOrDefault();
                if (details != null)
                {
                    Session["id_user"] = details.id_user;
                    Session["imie"] = details.imie;
                    return RedirectToAction("Start");
                }
                else
                {
                    ModelState.AddModelError("", "Niepoprawne dane!");
                }
            }
            return View();
        }

        [AllowAnonymous]
        public ActionResult Start()
        {
            return View();
        }
        [AllowAnonymous]
        public ActionResult Activation()
        {
            if(RouteData.Values["id"] != null)
            {
                Guid activationCode = new Guid(RouteData.Values["id"].ToString());
                KsiegarniaEntities ke = new KsiegarniaEntities();
                Aktywacja aktywacja = ke.Aktywacja.Where(p => p.kod == activationCode).FirstOrDefault();
                Uzytkownicy usr = ke.Uzytkownicy.Where(p => p.id_user == aktywacja.id).FirstOrDefault();

                if (aktywacja != null && usr != null)
                {
                    usr.aktywny = 1;
                    ke.Aktywacja.Remove(aktywacja);
                    ke.SaveChanges();
                    RedirectToAction("Index");
                }
            }
            return View();
        }

        private void SendActivationEmail(Uzytkownicy usr)
        {
            Guid activationCode = Guid.NewGuid();
            KsiegarniaEntities ke = new KsiegarniaEntities();
            ke.Aktywacja.Add(new Aktywacja
            {
                id = usr.id_user,
                kod = activationCode
            });
            ke.SaveChanges();

            using (MailMessage mm = new MailMessage("pikolo941@wp.pl", usr.email))
            {
                mm.Subject = "Aktywacja konta w serwisie Elektroniczna Księgarnia";
                string body = "Witaj " + usr.imie + ",";
                body += "<br /><br />Aby aktywować konto w Elektronicznej Księgarni kliknij poniższy link. ";
                body += "<br /><a href = '" + string.Format("{0}://{1}/Account/Activation/{2}", Request.Url.Scheme, Request.Url.Authority, activationCode) + "'>Kliknij aby aktywować.</a>";
                body += "<br /><br />Dziękujemy :)";
                mm.Body = body;
                mm.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.wp.pl";
                smtp.EnableSsl = true;
                NetworkCredential NetworkCred = new NetworkCredential("pikolo941@wp.pl", "pikolak94");
                smtp.UseDefaultCredentials = true;
                smtp.Credentials = NetworkCred;
                smtp.Port = 587;
                smtp.Send(mm);
            }
        }

        [AllowAnonymous]
        public ActionResult Welcome()
        {
            return View();
        }



        #region funkcje

        #endregion


    }
}