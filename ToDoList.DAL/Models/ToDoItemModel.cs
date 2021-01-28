using System.ComponentModel.DataAnnotations.Schema;
using ToDoList.Common.Enums;
using ToDoList.DAL.Models.Base;

namespace ToDoList.DAL.Models
{
    public class ToDoItemModel : BaseModel
    {
        public string Title { get; set; }

        public ToDoItemStatus Status { get; set; }

        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public UserModel User { get; set; }
    }
}
