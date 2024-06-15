using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyTasks.Models;
using System.Linq;
using System.Threading.Tasks;

namespace MyTasks.Controllers
{
    public class TaskAssignmentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TaskAssignmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TaskAssignments
        public async Task<IActionResult> Index()
        {
            var taskAssignments = _context.TaskAssignments
                                           .Include(t => t.Task)
                                           .Include(t => t.User);
            return View(await taskAssignments.ToListAsync());
        }

        // GET: TaskAssignments/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Name");
            ViewData["TaskId"] = new SelectList(_context.Tasks, "Id", "Title");
            return View();
        }

        // POST: TaskAssignments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,TaskId,AssignedDate")] TaskAssignment taskAssignment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(taskAssignment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Name", taskAssignment.UserId);
            ViewData["TaskId"] = new SelectList(_context.Tasks, "Id", "Title", taskAssignment.TaskId);
            return View(taskAssignment);
        }

        // GET: TaskAssignments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taskAssignment = await _context.TaskAssignments.FindAsync(id);
            if (taskAssignment == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Name", taskAssignment.UserId);
            ViewData["TaskId"] = new SelectList(_context.Tasks, "Id", "Title", taskAssignment.TaskId);
            return View(taskAssignment);
        }

        // POST: TaskAssignments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,TaskId,AssignedDate")] TaskAssignment taskAssignment)
        {
            if (id != taskAssignment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(taskAssignment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TaskAssignmentExists(taskAssignment.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Name", taskAssignment.UserId);
            ViewData["TaskId"] = new SelectList(_context.Tasks, "Id", "Title", taskAssignment.TaskId);
            return View(taskAssignment);
        }

        // GET: TaskAssignments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var taskAssignment = await _context.TaskAssignments
                            .Include(t => t.Task)
                            .Include(t => t.User)
                            .FirstOrDefaultAsync(m => m.Id == id);
            if (taskAssignment == null)
            {
                return NotFound();
            }

            return View(taskAssignment);
        }

        // POST: TaskAssignments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var taskAssignment = await _context.TaskAssignments.FindAsync(id);
            _context.TaskAssignments.Remove(taskAssignment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TaskAssignmentExists(int id)
        {
            return _context.TaskAssignments.Any(e => e.Id == id);
        }
    }
}