using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace MyTasks.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Display(Name = "Фамилия")]
        public string Surname { get; set; }

        [Display(Name = "Имя")]
        public string Name { get; set; }

        [Display(Name = "Отчество")]
        public string Patronymic { get; set; }

        [Display(Name = "Роль")]
        public string Role { get; set; }

        [Display(Name = "Пароль")]
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; internal set; }
       

    }
}
