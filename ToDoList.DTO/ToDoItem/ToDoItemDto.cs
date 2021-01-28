using ToDoList.DTO.Base;

namespace ToDoList.DTO.ToDoItem
{
    public class ToDoItemDto : BaseDto
    {
        public string Title { get; set; }

        public int StatusId { get; set; }

        public int UserId { get; set; }
    }
}
