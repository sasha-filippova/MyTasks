using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace MyTasks.Models
{
    public class Report
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
        [Display(Name = "Описание")]
        public string Description { get; set; }
        [Display(Name = "Дата создания")]
        public DateOnly CreateDate { get; set; }
        [Display(Name = "Содержание")]
        public string Text { get; set; }
        [Display(Name = "Проект")]
        public int ProjectId { get; set; }
        public Project Projects { get; set; }

    }
}
