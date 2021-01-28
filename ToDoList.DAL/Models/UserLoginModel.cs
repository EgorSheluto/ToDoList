using System;
using System.ComponentModel.DataAnnotations.Schema;
using ToDoList.DAL.Models.Base;

namespace ToDoList.DAL.Models
{
    public class UserLoginModel : BaseModel
    {
        public string RefreshToken { get; set; }

        public string UserAgent { get; set; }

        public DateTime ExpireDateUTC { get; set; }

        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public UserModel User { get; set; }
    }
}
