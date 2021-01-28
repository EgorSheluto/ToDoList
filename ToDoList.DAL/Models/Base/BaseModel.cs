using System;

namespace ToDoList.DAL.Models.Base
{
    public class BaseModel
    {
        public int Id { get; set; }

        public DateTime CreateDateUTC { get; set; }

        public DateTime? UpdateDateUTC { get; set; }
    }
}
