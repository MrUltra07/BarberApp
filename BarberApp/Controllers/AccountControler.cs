using System.Linq;
using System.Web.Mvc;
using BarberApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace BarberApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context = new AppDbContext();

        // GET: /Account/Login
        public ActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            // Kullanıcıyı bul
            var employee = _context.Employees.FirstOrDefault(e => e.Name == username && e.Password == password);

            if (employee != null)
            {
                // Kullanıcının Admin yetkisi var mı kontrol et
                var isAdmin = _context.EmployeeSkillLinkeds
                    .Any(esl => esl.PersonelId == employee.Id && esl.Skill.Title == "admin");

                // Session'a bilgileri ekle
                Session["EmployeeId"] = employee.Id;
                Session["EmployeeName"] = employee.Name;
                Session["IsAdmin"] = isAdmin;

                // Yönlendir
                if (isAdmin)
                {
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    return RedirectToAction("Index", "Employee");
                }
            }
            else
            {
                ViewBag.ErrorMessage = "Geçersiz kullanıcı adı veya şifre.";
                return View();
            }
        }

        // GET: /Account/Logout
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }

        // GET: /Account/AccessDenied
        public ActionResult AccessDenied()
        {
            return View();
        }
    }
}
