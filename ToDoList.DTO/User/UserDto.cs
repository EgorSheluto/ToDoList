using ToDoList.DTO.Base;

namespace ToDoList.DTO.User
{
    public class UserDto : BaseDto
    {
        public string Login { get; set; }

        public string Password { get; set; }
    }
}
