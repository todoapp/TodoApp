namespace TodoApp.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using TodoApp.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Microsoft.AspNet.Identity.Owin;

    internal sealed class Configuration : DbMigrationsConfiguration<TodoApp.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "TodoApp.Models.ApplicationDbContext";
        }

        protected override void Seed(TodoApp.Models.ApplicationDbContext context)
        {
            if (!context.Users.Any(u => u.UserName == "steve@example.com"))
            {
                var userStore = new UserStore<ApplicationUser>(context);
                var userManager = new UserManager<ApplicationUser>(userStore);
                var user = new ApplicationUser() { UserName = "steve@example.com", Email = "steve@example.com" };
                userManager.Create(user, "Password1!");

                context.TodoItems.AddOrUpdate(x => x.Id,
                    new TodoItem() { Id = 1, Name = "Buy Milk", ApplicationUserId = user.Id },
                    new TodoItem() { Id = 2, Name = "Buy Bananas", ApplicationUserId = user.Id },
                    new TodoItem() { Id = 3, Name = "Buy Cheese", ApplicationUserId = user.Id }
                );
            }
        }
    }
}
