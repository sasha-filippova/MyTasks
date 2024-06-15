using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyTasks.Models;
using System.Linq;
using System.Threading.Tasks;

namespace MyTasks.Controllers
{
    [Authorize(Roles = "Employee")]
    public class EmployeeController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public EmployeeController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // Главная страница для обычного пользователя
        public IActionResult Index()
        {
            var user = _userManager.GetUserAsync(User).Result;

            var userTasks = _context.TaskAssignments
                .Where(t => t.UserId == user.Id)
                .Select(t => t.Task)
                .ToList();

            return View(userTasks);
        }

        // Страница управления отчетами
        public IActionResult Reports()
        {
            return RedirectToAction("Index", "Reports");
        }

        // Страница управления задачами
        public IActionResult Tasks()
        {
            return RedirectToAction("Index", "Tasks");
        }

        public async Task<IActionResult> Search(string query)
        {
            var tasks = await _context.Tasks
                .Where(t => t.Title.Contains(query) || t.Description.Contains(query))
                .ToListAsync();
            return View(tasks);
        }
    }
}