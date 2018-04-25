using System.Web.Mvc;
using System.Web.Routing;

namespace WEB.Controllers
{
    [Authorize]
    public class HomeController : BaseMvcController
    {
        public ActionResult Index()
        {
            return View();
        }

        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
            // settings/dbcontext (httpcontext) are only available during initialize, not constructor
            ViewBag.SiteName = Settings.SiteName;
        }
    }
}