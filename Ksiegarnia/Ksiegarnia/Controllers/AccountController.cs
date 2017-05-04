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
        KsiegarniaEntities1 db = new KsiegarniaEntities1();

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
        public ActionResult Register(Klienci usr)
        {
            if(ModelState.IsValid)
            {
                var details = db.Klienci.Where(p => p.email == usr.email).FirstOrDefault();
                if (details == null)
                {
                    db.Klienci.Add(usr);
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
        public ActionResult Login(Klienci usr)
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
                var details = db.Klienci.Where(a => a.email.Equals(usr.email) && a.haslo.Equals(usr.haslo) && a.aktywny == 1).FirstOrDefault();
                if (details != null)
                {
                    Session["id_user"] = details.id_klient;
                    Session["imie"] = details.imie;
                    Session["koszyk"] = 0;
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
        public ActionResult LogOut()
        {
            Session.Clear();
            return RedirectToAction("Index", "Home");
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
                KsiegarniaEntities1 ke = new KsiegarniaEntities1();
                Aktywacja aktywacja = ke.Aktywacja.Where(p => p.kod == activationCode).FirstOrDefault();
                Klienci usr = ke.Klienci.Where(p => p.id_klient == aktywacja.id).FirstOrDefault();

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

        private void SendActivationEmail(Klienci usr)
        {
            Guid activationCode = Guid.NewGuid();
            KsiegarniaEntities1 ke = new KsiegarniaEntities1();
            ke.Aktywacja.Add(new Aktywacja
            {
                id = usr.id_klient,
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