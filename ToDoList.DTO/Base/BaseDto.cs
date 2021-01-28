using System;

namespace ToDoList.DTO.Base
{
    public class BaseDto
    {
        public int Id { get; set; }

        public DateTime CreateDateUTC { get; set; }

        public DateTime? UpdateDateUTC { get; set; }
    }
}
