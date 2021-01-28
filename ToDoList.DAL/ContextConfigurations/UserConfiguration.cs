using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ToDoList.DAL.ContextConfigurations.Base;
using ToDoList.DAL.Models;

namespace ToDoList.DAL.ContextConfigurations
{
    public class UserConfiguration : IEntityTypeConfiguration<UserModel> //BaseConfiguration<UserModel>
    {
        public virtual void Configure(EntityTypeBuilder<UserModel> builder)
        {
            //base.Configure(builder);
            builder?.ToTable("Users");

            builder.Property(p => p.Login).IsRequired()
                                          .HasMaxLength(50);
            builder.Property(p => p.Password).IsRequired()
                                             .HasMaxLength(25);
        }
    }
}
