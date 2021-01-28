using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ToDoList.DAL.Models.Base;

namespace ToDoList.DAL.ContextConfigurations.Base
{
    public class BaseConfiguration<T> : IEntityTypeConfiguration<T> where T : BaseModel
    {
        public virtual void Configure(EntityTypeBuilder<T> builder)
        {
            builder.Property(p => p.CreateDateUTC).IsRequired();
        }
    }
}
