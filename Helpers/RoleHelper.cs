using BarberApp.Models;

namespace BarberApp.Helpers
{
    public static class RoleHelper
    {
        public static bool IsAdmin(AppDbContext context, int employeeId)
        {
            return context.Employees
                .Any(employee => employee.Id == employeeId &&
                                 employee.Skills.Any(skill => skill.Title == "ADMIN"));
        }
    }
}
