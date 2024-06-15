using System.ComponentModel.DataAnnotations;

namespace MyTasks.Models
{
    public class Task
    {
        public int Id { get; set; }
        [Display(Name = "Название")]
        public string Title { get; set; }
        [Display(Name = "Описание")]
        public string Description { get; set; }
        [Display(Name = "Срок")]
        public DateOnly DueDate { get; set; }
        [Display(Name = "Приоритет")]
        public string Priority { get; set; }
        [Display(Name = "Статус")]
        public string Status { get; set; }
        [Display(Name = "Проект")]
        public int ProjectId { get; set; }
        public virtual Project Project { get; set; }
        public virtual ICollection<TaskAssignment> TaskAssignments { get; set; }
    }
}
