using Microsoft.AspNetCore.Mvc;
using BarberApp.Models;
using BarberApp.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace BarberApp.Controllers.Admin
{
	[Route("admin")]
	public class EmployeeController : Controller
	{
		private readonly AppDbContext _context;
		private static readonly string  viewAdress = "~/Views/Admin/Employee/";
		public EmployeeController(AppDbContext context)
		{
			_context = context;
		}

		[HttpGet("employees")]
		public IActionResult Employees()
		{
			var employees = _context.Employees.Include(e => e.Skills).ToList(); // Tüm çalışanları getir
			return View(viewAdress+"Index.cshtml", employees); // Employees/Index.cshtml'e gönder
		}
		[HttpGet("employees/create")]
		public IActionResult CreateEmployee()
		{
			var viewModel = new EmployeeViewModel
			{
				Employee = new Employee
				{
					Name = string.Empty,
					Surname = string.Empty,
					ProfileImageUrl = string.Empty,
					IdNumber = string.Empty,
					Iban = string.Empty,
					BasicWage = 0,
					Password = string.Empty
				},
				Skills = _context.Skills.ToList(),
				SelectedSkillIds = new List<int>()
			};

			return View(viewAdress + "Create/Index.cshtml", viewModel); // Create Employee view'i göster
		}

		[HttpPost("employees/create")]
		public IActionResult CreateEmployee(EmployeeViewModel viewModel)
		{
			if (ModelState.IsValid)
			{
				var newEmployee = viewModel.Employee;

				// Seçilen yetenekleri ekle
				foreach (var skillId in viewModel.SelectedSkillIds)
				{
					var skill = _context.Skills.FirstOrDefault(s => s.Id == skillId);
					if (skill != null)
					{
						newEmployee.Skills.Add(skill);
					}
				}

				_context.Employees.Add(newEmployee);
				_context.SaveChanges();

				return RedirectToAction("Employees", new { status = "ok", message = "Employee created successfully!" });
			}

			// Eğer ModelState geçerli değilse Skills'i yeniden doldurun
			viewModel.Skills = _context.Skills.ToList();
			return View(viewAdress + "Create/Index.cshtml", viewModel);
		}



		[HttpGet("employees/edit/{id}")]
		public IActionResult EditEmployee(int id)
		{
			var employee = _context.Employees
				.Include(e => e.Skills)
				.FirstOrDefault(e => e.Id == id);

			if (employee == null)
			{
				return NotFound();
			}

			var viewModel = new EmployeeViewModel
			{
				Employee = employee,
				Skills = _context.Skills.ToList(),
				SelectedSkillIds = employee.Skills.Select(s => s.Id).ToList()
			};

			return View(viewAdress + "Edit/Index.cshtml", viewModel);
		}

		[HttpPost("employees/edit/{id}")]
		public IActionResult EditEmployee(int id, EmployeeViewModel viewModel)
		{
			if (viewModel.SelectedSkillIds == null || !viewModel.SelectedSkillIds.Any())
			{
				TempData["Status"] = "error";
				TempData["Message"] = "Please select at least one skill.";
				return RedirectToAction("Employees");
			}

			var employee = _context.Employees
				.Include(e => e.Skills)
				.FirstOrDefault(e => e.Id == id);

			if (employee == null)
			{
				TempData["Status"] = "error";
				TempData["Message"] = "Employee not found.";
				return RedirectToAction("Employees");
			}

			// Çalışan bilgilerini güncelleme
			employee.Name = viewModel.Employee.Name;
			employee.Surname = viewModel.Employee.Surname;
			employee.ProfileImageUrl = viewModel.Employee.ProfileImageUrl;
			employee.IdNumber = viewModel.Employee.IdNumber;
			employee.Iban = viewModel.Employee.Iban;
			employee.BasicWage = viewModel.Employee.BasicWage;
			employee.Password = viewModel.Employee.Password;

			// Skill'leri güncelleme
			employee.Skills.Clear();
			foreach (var skillId in viewModel.SelectedSkillIds)
			{
				var skill = _context.Skills.FirstOrDefault(s => s.Id == skillId);
				if (skill != null)
				{
					employee.Skills.Add(skill);
				}
			}

			_context.SaveChanges();
			TempData["Status"] = "ok";
			TempData["Message"] = "Employee updated successfully!";
			return RedirectToAction("Employees");
		}



		[HttpGet("employees/delete/{id}")]
		public IActionResult DeleteEmployee(int id)
		{
			var employee = _context.Employees.FirstOrDefault(e => e.Id == id);
			if (employee == null) return NotFound();

			_context.Employees.Remove(employee);
			_context.SaveChanges();

			return RedirectToAction("Employees", new { status = "ok", message = "Employee deleted successfully!" });
		}
	}
}