using System.ComponentModel.DataAnnotations;

namespace MyTasks.Models
{
    public class TaskAssignment
    {
        public int Id { get; set; }
        [Display(Name = "Задача")]
        public int TaskId { get; set; }
        public virtual Task Task { get; set; }
        [Display(Name = "Пользователь")]
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
        [Display(Name = "Дата назначения")]
        public DateTime AssignedDate { get; set; }
    }
}
