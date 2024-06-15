using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyTasks.Models;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MyTasks.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _logger = logger;
            _userManager = userManager;
            _context = context;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                ViewBag.UserName = user.UserName;
                var userRoles = await _userManager.GetRolesAsync(user);
                ViewBag.Role = userRoles;

                IQueryable<Models.Task> tasksQuery = _context.Tasks
                    .Include(t => t.TaskAssignments)
                    .ThenInclude(ta => ta.User);

                if (userRoles.Contains("Manager"))
                {
                    // Менеджер видит все задачи
                    tasksQuery = tasksQuery.Where(t => t.TaskAssignments.Any());
                }
                else
                {
                    // Администратор и сотрудник видят только свои задачи
                    tasksQuery = tasksQuery.Where(t => t.TaskAssignments.Any(ta => ta.UserId == user.Id));
                }

                var tasks = await tasksQuery.ToListAsync();

                return View(tasks);
            }

            return View(new List<Models.Task>());
        }

        public async Task<IActionResult> Search(string query)
        {
            var tasks = await _context.Tasks
                .Where(t => t.Title.Contains(query) || t.Description.Contains(query))
                .ToListAsync();
            return View(tasks);
        }

        [Authorize(Roles = "Admin,Manager,Employee")]
        public IActionResult Privacy()
        {
            return View();
        }

        [Authorize(Roles = "Admin,Manager")]
        public IActionResult AdminAndManagerOnly()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        public IActionResult AdminOnly()
        {
            return View();
        }

        [Authorize(Roles = "Manager")]
        public IActionResult ManagerOnly()
        {
            return View();
        }

        [Authorize(Roles = "Employee")]
        public IActionResult EmployeeOnly()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}