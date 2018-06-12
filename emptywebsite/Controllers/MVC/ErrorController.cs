using System;
using System.Web.Mvc;
using WEB.Models;

namespace WEB.Controllers.MVC
{
    [AuthorizeRoles(Roles.Administrator)]
    public class ErrorController : BaseMvcController
    {
        public ActionResult Index(int? id)
        {
            if (id.HasValue) Response.StatusCode = Convert.ToInt32(id);
            return View();
        }
    }
}
