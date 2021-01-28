using System.ComponentModel.DataAnnotations;

namespace ToDoList.MVC.Models
{
    public class ToDoAddModel
    {
        [Required]
        [MinLength(4), MaxLength(50)]
        public string Title { get; set; }
    }
}
