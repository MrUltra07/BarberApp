using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Web.Mvc;

namespace BarberApp.Controllers
{
    public class AdminController : BaseController
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            // Kullanıcının Admin yetkisi var mı kontrol et
            if (Session["IsAdmin"] == null || !(bool)Session["IsAdmin"])
            {
                filterContext.Result = RedirectToAction("AccessDenied", "Account");
            }
        }

        // GET: /Admin/Index
        public ActionResult Index()
        {
            // Admin ana sayfası
            return View();
        }

        // Diğer admin işlemleri...
    }
}
