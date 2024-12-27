using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
namespace BarberApp.Middlewares;
public class RoleBasedAuthorizationMiddleware
{
    private readonly RequestDelegate _next;

    public RoleBasedAuthorizationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var employeeId = context.Session.GetString("EmployeeId");
        var isAdmin = context.Session.GetString("IsAdmin") == "true";
        var isEmployee = context.Session.GetString("IsEmployee") == "true";
        var path = context.Request.Path;

        // Login yoluna herkes erişebilir
        if (path.StartsWithSegments("/admin/login"))
        {
            await _next(context);
            return;
        }

       

        // Admin ve çalışanların erişebileceği yollar
        if (path.StartsWithSegments("/admin/appointments") || path.StartsWithSegments("/admin/dashboard"))
        {
            await _next(context);
            return; // Erişim izni ver, devam et
        }

        // Diğer admin yolları için sadece adminler erişebilir
        if (path.StartsWithSegments("/admin/settings") || path.StartsWithSegments("/admin/users") || path.StartsWithSegments("/admin/slider") ||
            path.StartsWithSegments("/admin/invoice") || path.StartsWithSegments("/admin/skills") || path.StartsWithSegments("/admin/employees") || path.StartsWithSegments("/admin/generalSettings") ||
            path.StartsWithSegments("/admin/availability"))
        {
            if (!isAdmin) // Eğer kullanıcı admin değilse
            {
                context.Response.Redirect("/admin/login?"+isAdmin);
                return;
            }
        }

        await _next(context);
    }

}
