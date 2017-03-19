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
                db.Uzytkownicy.Add(usr);
                db.SaveChanges();
                SendActivationEmail(usr);
                return RedirectToAction("Welcome");
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
            if(ModelState.IsValid)
            {
                var details = db.Uzytkownicy.Where(a => a.email.Equals(usr.email) && a.haslo.Equals(usr.haslo)).FirstOrDefault();
                if (details != null)
                    return RedirectToAction("Welcome");
            }
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
                if (aktywacja != null)
                {
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

            using (MailMessage mm = new MailMessage("marcin.mackowiak@windowlive.com", usr.email))
            {
                mm.Subject = "Aktywacja konta w serwisie Elektroniczna Księgarnia";
                string body = "Witaj " + usr.imie + ",";
                body += "<br /><br />Aby aktywować konto w Elektronicznej Księgarni kliknij poniższy link. ";
                body += "<br /><a href = '" + string.Format("{0}://{1}/Account/Activation/{2}", Request.Url.Scheme, Request.Url.Authority, activationCode) + "'>Kliknij aby aktywować.</a>";
                body += "<br /><br />Dziękujemy :)";
                mm.Body = body;
                mm.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.live.com";
                smtp.EnableSsl = true;
                NetworkCredential NetworkCred = new NetworkCredential("marcin.mackowiak@windowslive.com", "Mackowiak$M9404");
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