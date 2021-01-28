using Microsoft.EntityFrameworkCore;
using System.Reflection;
using ToDoList.DAL.Models;

namespace ToDoList.DAL.Contexts
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options)
            : base(options)
        {

        }

        public DbSet<ToDoItemModel> ToDoItems { get; set; }

        public DbSet<UserModel> Users { get; set; }

        public DbSet<UserLoginModel> UsersLogins { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Almost all configs for db tables is in the context configs because for me it's more flexible and scalable
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
