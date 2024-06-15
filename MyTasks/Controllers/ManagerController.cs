using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyTasks.Models;
using System.Linq;
using System.Threading.Tasks;

namespace MyTasks.Controllers
{
    [Authorize(Roles = "Manager")]
    public class ManagerController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public ManagerController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // Главная страница для руководителя
        public async Task<IActionResult> Index()
        {
            // Загрузка задач с включением связанных данных
            var tasks = await _context.Tasks
                .Include(t => t.Project)
                .Include(t => t.TaskAssignments)
                .ThenInclude(ta => ta.User)
                .Take(10) // Пример получения первых 10 задач
                .ToListAsync();

            return View(tasks);
        }

        // Страница управления пользователями
        public IActionResult Users()
        {
            var users = _userManager.Users.ToList();
            return View(users);
        }

        // Использование существующего представления проектов
        public IActionResult Projects()
        {
            return RedirectToAction("Index", "Projects");
        }

        // Использование существующего представления задач
        public IActionResult Tasks()
        {
            return RedirectToAction("Index", "Tasks");
        }

        // Использование существующего представления отчетов
        public IActionResult Reports()
        {
            return RedirectToAction("Index", "Reports");
        }

        // GET: Manager/AssignTask
        public IActionResult AssignTask()
        {
            ViewBag.Users = _userManager.Users.ToList();
            ViewBag.Projects = _context.Projects.ToList();
            ViewBag.Tasks = _context.Tasks.ToList();
            return View();
        }

        // POST: Manager/AssignTask
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignTask(TaskAssignment model)
        {
            if (ModelState.IsValid)
            {
                _context.TaskAssignments.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Users = _userManager.Users.ToList();
            ViewBag.Projects = _context.Projects.ToList();
            ViewBag.Tasks = _context.Tasks.ToList();
            return View(model);
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