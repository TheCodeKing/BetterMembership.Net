using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BetterMembership.Website.Controllers
{
    using System.Web.Profile;

    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";

            if (this.ControllerContext.HttpContext.User.Identity.IsAuthenticated)
            {
                //var profile = ProfileBase.Create(this.ControllerContext.HttpContext.User.Identity.Name);
                //var value = profile["ProfileColumn"];
                //profile["ProfileColumn"] = "new value";
                //profile.Save();
            }

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
