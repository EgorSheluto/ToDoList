using System.ComponentModel.DataAnnotations;

namespace ToDoList.MVC.Models
{
    public class LoginModel
    {
        [Required]
        //[StringLength(50)]
        [MinLength(4), MaxLength(50)]
        public string Login { get; set; }

        [Required]
        //[StringLength(25)]
        [MinLength(4), MaxLength(25)]
        public string Password { get; set; }
    }
}
