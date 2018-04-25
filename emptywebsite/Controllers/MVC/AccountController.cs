using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using WEB.Models;
using System.Net.Mail;
using System.Configuration;
using System.Linq;
using WEB.Utilities;
using System.Web.Routing;

namespace WEB.Controllers
{
    [AllowAnonymous]
    public class AccountController : BaseMvcController
    {
        private ApplicationSignInManager _signInManager;

        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
            // settings/dbcontext (httpcontext) are only available during initialize, not constructor
            ViewBag.SiteName = Settings.SiteName;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        //
        // GET: /login
        // prevent antiforgery token issues on back button
        [OutputCache(NoStore = true, Location = System.Web.UI.OutputCacheLocation.None)]
        public ActionResult Login()
        {
            // todo: should check the hostname here that it exists in the current valid clients list (central database)
            // todo: also check in other account options, such as password reset, etc. (so it doesn't create db on a request)
            return View();
        }

        //
        // POST: /login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, true, shouldLockout: true);
            switch (result)
            {
                case SignInStatus.Success:
                    var user = db.Users.Single(o => o.Email == model.Email);
                    throw new Exception("TODO: Add an Enabled field to the User Class/Entity, or remove/modify the next line");
                    //if (!user.Enabled)
                    //{
                    //    ModelState.AddModelError("", "Your account has been disabled.");
                    //    return View(model);
                    //}
                    return RedirectToAction("Index", "Home");
                case SignInStatus.LockedOut:
                    return RedirectToAction("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login.");
                    return View(model);
            }
        }

        //
        // GET: /lockout
        public ActionResult Lockout()
        {
            return View();
        }

        //
        // GET: /resetpassword
        // prevent antiforgery token issues on back button
        [OutputCache(NoStore = true, Location = System.Web.UI.OutputCacheLocation.None)]
        public ActionResult ResetPassword()
        {
            return View();
        }

        // POST: /resetpassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await UserManager.FindByNameAsync(model.Email);

            if (user == null)
            {
                ModelState.AddModelError("", "Invalid email address.");
                return View(model);
            }

            // todo: locked out & not active:
            //if (!user.LockoutEndDateUtc)
            //{
            //    ModelState.AddModelError("", "Invalid email address.");
            //    return View(model);
            //}

            var provider = new MachineKeyProtectionProvider();
            UserManager.UserTokenProvider = new DataProtectorTokenProvider<User, Guid>(provider.Create("PasswordReset"));
            var resetToken = await UserManager.GeneratePasswordResetTokenAsync(user.Id);

            var rootUrl = ConfigurationManager.AppSettings["RootUrl"];
            var message = new MailMessage();
            message.To.Add(new MailAddress(user.Email));
            message.Subject = "Password Reset";
            throw new Exception("TODO: Add a FirstName to the User Class/Entity, or remove/modify the next line");
            //message.Body = user.FirstName + Environment.NewLine;
            message.Body += Environment.NewLine;
            message.Body += "A password reset has been requested. Please use the link below to reset your password." + Environment.NewLine;
            message.Body += Environment.NewLine;
            message.Body += rootUrl + "reset?e=" + user.Email + "&t=" + HttpUtility.UrlEncode(resetToken) + Environment.NewLine;

            Email.SendMail(message, Settings);

            return RedirectToAction("Login", "Account", new { msg = "tokensent" });

        }

        //
        // GET: /reset
        // prevent antiforgery token issues on back button
        [OutputCache(NoStore = true, Location = System.Web.UI.OutputCacheLocation.None)]
        public ActionResult Reset()
        {
            var model = new ResetViewModel { Email = Request.QueryString["e"], Token = Request.QueryString["t"] };
            return View(model);
        }

        // POST: /reset
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Reset(ResetViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var provider = new MachineKeyProtectionProvider();
            UserManager.UserTokenProvider = new DataProtectorTokenProvider<User, Guid>(provider.Create("PasswordReset"));
            var user = await UserManager.FindByNameAsync(model.Email);

            if (user == null)
            {
                ModelState.AddModelError("", "Invalid email address.");
                return View(model);
            }

            // todo: locked out & not active:
            //if (!user.LockoutEndDateUtc)
            //{
            //    ModelState.AddModelError("", "Invalid email address.");
            //    return View(model);
            //}

            var result = await UserManager.ResetPasswordAsync(user.Id, model.Token, model.Password);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", result.Errors.First());
                return View(model);
            }

            var message = new MailMessage();
            message.To.Add(new MailAddress(user.Email));
            message.Subject = "Password Changed";
            throw new Exception("TODO: Add a FirstName to the User Class/Entity, or remove/modify the next line");
            //message.Body = user.FirstName + Environment.NewLine;
            message.Body += Environment.NewLine;
            message.Body += "Your password has been changed." + Environment.NewLine;

            Utilities.Email.SendMail(message, Settings);

            if (!user.EmailConfirmed) user.EmailConfirmed = true;
            await UserManager.UpdateAsync(user);

            return RedirectToAction("Login", "Account", new { msg = "passwordchanged" });
        }

        //
        // GET: /logout
        public ActionResult Logout()
        {
            Request.GetOwinContext().Authentication.SignOut();
            Session.Abandon();
            return RedirectToAction("Login", "Account", new { msg = "loggedout" });
        }

        #region Helpers
        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }
        #endregion
    }
}