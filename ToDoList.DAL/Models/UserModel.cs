using System.Collections.Generic;
using ToDoList.DAL.Models.Base;

namespace ToDoList.DAL.Models
{
    public class UserModel : BaseModel
    {
        public string Login { get; set; }

        // I prefer use HashPassword but it costs time
        public string Password { get; set; }

        public List<UserLoginModel> UsersLogins { get; set; } = new List<UserLoginModel>();

        public List<ToDoItemModel> ToDoItems { get; set; } = new List<ToDoItemModel>();
    }
}
