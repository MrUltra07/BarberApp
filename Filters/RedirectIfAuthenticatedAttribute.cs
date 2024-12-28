using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BarberApp.Filters // Projenizin namespace'ini ayarlayın
{
	public class RedirectIfAuthenticatedAttribute : ActionFilterAttribute
	{
		public override void OnActionExecuting(ActionExecutingContext context)
		{
			var isUserLoggedIn = context.HttpContext.Session.GetString("CustomerId") != null;
			if (isUserLoggedIn)
			{
				context.Result = new RedirectToActionResult("Index", "Home", null);
			}
			base.OnActionExecuting(context);
		}
	}
}
