using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace MyTasks.Models
{
    public class CreateUserViewModel
    {
        [Display(Name = "Логин")]
        [Required]
        public string UserName { get; set; }

        [Display(Name = "Фамилия")]
        [Required]
        public string Surname { get; set; }

        [Display(Name = "Имя")]
        [Required]
        public string Name { get; set; }

        [Display(Name = "Отчество")]
        public string Patronymic { get; set; }

        [Display(Name = "Пароль")]
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Роль")]
        [Required]
        public string Role { get; set; }
        public List<SelectListItem> Roles { get; set; }
        [Display(Name = "Электронная почта")]
        [Required]
        public string Email { get; set; }
    }
}
