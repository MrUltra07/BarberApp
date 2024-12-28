namespace BarberApp.Controllers;

using BarberApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class BaseController : Controller
{
	protected readonly AppDbContext _context;

	public BaseController(AppDbContext context)
	{
		_context = context;
	}

	public override void OnActionExecuting(ActionExecutingContext context)
	{
		var settings = _context.GeneralSettings.FirstOrDefault(g => g.Id == 1);
		if (settings != null)
		{
			ViewData["GeneralSettings"] = settings;
		}

		base.OnActionExecuting(context);
	}
}
