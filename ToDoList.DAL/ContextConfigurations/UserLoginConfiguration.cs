using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ToDoList.DAL.Models;

namespace ToDoList.DAL.ContextConfigurations
{
    public class UserLoginConfiguration : IEntityTypeConfiguration<UserLoginModel>
    {
        public virtual void Configure(EntityTypeBuilder<UserLoginModel> builder)
        {
            //base.Configure(builder);
            builder?.ToTable("UsersLogins");

            builder.Property(p => p.ExpireDateUTC).IsRequired();
            builder.Property(p => p.RefreshToken).IsRequired()
                                                 .HasMaxLength(350);
            builder.Property(p => p.UserAgent).IsRequired()
                                              .HasMaxLength(500);

            builder.HasOne(p => p.User)
                   .WithMany(p => p.UsersLogins)
                   .HasForeignKey(p => p.UserId);
        }
    }
}
