using System.ComponentModel.DataAnnotations;

namespace MyTasks.Models
{
    public class Project
    {
        public int Id { get; set; }

        [Display(Name = "Название проекта")]
        public string Title { get; set; }

        [Display(Name = "Описание")]
        public string Description { get; set; }

        [Display(Name = "Дата начала")]
        public DateOnly StartDate { get; set; }

        [Display(Name = "Дата окончания")]
        public DateOnly EndDate { get; set; }

        [Display(Name = "Статус")]
        public string Status { get; set; }
    }
}
