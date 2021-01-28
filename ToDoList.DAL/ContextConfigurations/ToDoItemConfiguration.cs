using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ToDoList.Common.Enums;
using ToDoList.DAL.Models;

namespace ToDoList.DAL.ContextConfigurations
{
    public class ToDoItemConfiguration : IEntityTypeConfiguration<ToDoItemModel>
    {
        public virtual void Configure(EntityTypeBuilder<ToDoItemModel> builder)
        {
            //base.Configure(builder);
            builder?.ToTable("ToDoItems");

            builder.Property(p => p.Title).IsRequired()
                                          .HasMaxLength(300);
            builder.Property(p => p.Status).IsRequired()
                                           .HasDefaultValue(ToDoItemStatus.New);

            builder.HasOne(p => p.User)
                   .WithMany(p => p.ToDoItems)
                   .HasForeignKey(p => p.UserId);
        }
    }
}
