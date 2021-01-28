using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using ToDoList.DAL.Models;

namespace ToDoList.DAL.ContextConfigurations
{
    public class UserConfiguration : IEntityTypeConfiguration<UserModel>
    {
        public virtual void Configure(EntityTypeBuilder<UserModel> builder)
        {
            builder?.ToTable("Users");

            builder.Property(p => p.Login).IsRequired()
                                          .HasMaxLength(50);
            builder.Property(p => p.Password).IsRequired()
                                             .HasMaxLength(25);

            builder.HasData(new List<UserModel>
            {
                new UserModel
                {
                    Id = 1,
                    Login = "root",
                    Password = "root"
                },
                new UserModel
                {
                    Id = 2,
                    Login = "roots",
                    Password = "roots"
                }
            });
        }
    }
}
