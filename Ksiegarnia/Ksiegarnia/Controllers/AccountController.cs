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
        public ActionResult Register(Uzytkownicy usr, Adresy adr)
        {
            if(ModelState.IsValid)
            {
                db.Adresy.Add(adr);
                db.Uzytkownicy.Add(usr);
                db.SaveChanges();
                return RedirectToAction("Index");
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
                var details = db.Uzytkownicy.Where(a => a.email.Equals(usr.email) && a.haslo.Equals(usr.haslo) && a.aktywny.Equals(1)).FirstOrDefault();
                if (details != null)
                    return RedirectToAction("Index");
            }
            return View();
        }
        #region funkcje

        private void SendActivationEmail(int userId, string mail, string imie)
        {
            string activationCode = Guid.NewGuid().ToString();
            var aktyw = new Aktywacja { id = userId, kod = Guid.Parse(activationCode) };
            db.Aktywacja.Add(aktyw);
            db.SaveChanges();

            using (MailMessage mm = new MailMessage("marcin.mackowiak@windowslive.com", mail))
            {
                mm.Subject = "Aktywacja konta w elektronicznej księgarni";
                string body = "Witaj " + imie + "!";
                body += "<br /><br />Aby aktywować konto w serwisie kliknij poniższy link";
                body += "<br /><a href = '" + Request.Url.AbsoluteUri.Replace("CS.aspx", "CS_Activation.aspx?ActivationCode=" + activationCode) + "'>Click here to activate your account.</a>";
                body += "<br /><br />Dziękujemy.";
                mm.Body = body;
                mm.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.live.com";
                smtp.EnableSsl = true;
                NetworkCredential NetworkCred = new NetworkCredential("marcin.mackowiak@windowslive.com", "<password>");
                smtp.UseDefaultCredentials = true;
                smtp.Credentials = NetworkCred;
                smtp.Port = 587;
                smtp.Send(mm);
            }
        }

        protected void pageLoad(object sender, EventArgs e)
        {
            
        }
        #endregion


    }
}