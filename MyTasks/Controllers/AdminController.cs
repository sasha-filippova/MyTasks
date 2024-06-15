using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyTasks.Models;
using System.Linq;
using System.Threading.Tasks;

namespace MyTasks.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public AdminController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // Главная страница для администратора
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("User not found");
            }

            var userRoles = await _userManager.GetRolesAsync(user);

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

            var tasks = await tasksQuery.Take(10).ToListAsync(); // Пример получения первых 10 задач
            return View(tasks);
        }

        // Страница управления пользователями
        public IActionResult Users()
        {
            var users = _userManager.Users.ToList();
            return View(users);
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

        // GET: Admin/CreateUser
        public IActionResult CreateUser()
        {
            var model = new CreateUserViewModel
            {
                Roles = GetRolesSelectList()
            };
            return View(model);
        }

        // POST: Admin/CreateUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser(CreateUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.UserName,
                    Surname = model.Surname,
                    Name = model.Name,
                    Patronymic = model.Patronymic,
                    Email = model.Email
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(model.Role))
                    {
                        await _userManager.AddToRoleAsync(user, model.Role);
                    }
                    else
                    {

                    }
                    return RedirectToAction(nameof(Users));
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            // Повторно передаем список ролей в случае ошибки
            model.Roles = GetRolesSelectList();
            return View(model);
        }

        private List<SelectListItem> GetRolesSelectList()
        {
            // Пример получения списка ролей
            return new List<SelectListItem>
        {
            new SelectListItem { Value = "Admin", Text = "Admin" },
            new SelectListItem { Value = "Manager", Text = "Manager" },
            new SelectListItem { Value = "Employee", Text = "Employee" }
        };
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