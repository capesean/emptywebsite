using WEB.Models;
using System.Web;
using Microsoft.AspNet.Identity.Owin;
using System.Web.Mvc;
using System.Web.Routing;
using System.Linq;
using Microsoft.AspNet.Identity;

namespace WEB.Controllers  
{
    [HandleError]
    public class BaseMvcController : Controller
    {
        private AppUserManager _userManager;
        private ApplicationDbContext _db;
        private User _currentUser;
        private Settings _settings;
        internal AppUserManager UserManager
        {
            get
            {
                if (_userManager == null) _userManager = HttpContext.GetOwinContext().GetUserManager<AppUserManager>();
                return _userManager;
            }
        }
        internal ApplicationDbContext db
        {
            get
            {
                // set in initialise
                return _db;
            }
        }
        public Settings Settings
        {
            get
            {
                if (_settings == null) _settings = new Settings(db);
                return _settings;
            }
        }
        internal User CurrentUser
        {
            get
            {
                if (_currentUser == null) _currentUser = UserManager.FindByNameAsync(User.Identity.Name).Result;
                return _currentUser;
            }
        }

        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
            _db = requestContext.HttpContext.GetOwinContext().Get<ApplicationDbContext>();
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            Utilities.ErrorLogger.Log(filterContext.Exception, System.Web.HttpContext.Current.Request, filterContext.HttpContext.Request.Url.ToString(), filterContext.HttpContext.User.Identity.Name);
            base.OnException(filterContext);
        }

        public class AuthorizeRolesAttribute : AuthorizeAttribute
        {
            public AuthorizeRolesAttribute(params Roles[] roles) : base()
            {
                Roles = string.Join(",", roles.Select(r => r.ToString()));
            }
        }

        internal bool CurrentUserIsInRole(Roles role)
        {
            return UserManager.IsInRole(CurrentUser.Id, role.ToString());
        }
    }
}