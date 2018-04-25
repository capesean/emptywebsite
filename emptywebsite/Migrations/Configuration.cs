namespace WEB.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System.Collections.Generic;

    internal sealed class DevelopmentConfiguration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        public DevelopmentConfiguration()
        {
            ContextKey = "WEB.Models.ApplicationDbContext";
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(ApplicationDbContext context)
        {
            base.Seed(context);

            context.AddComputedColumns();

            using (var roleStore = new RoleStore<AppRole, Guid, AppUserRole>(context))
            using (var roleManager = new RoleManager<AppRole, Guid>(roleStore))
            using (var userManager = new AppUserManager(new AppUserStore(context)))
            {
                var allRoles = Enum.GetNames(typeof(Roles)).ToList();
                foreach (var role in allRoles)
                {
                    if (!roleManager.RoleExists(role.ToLower()))
                    {
                        roleManager.Create(new AppRole() { Name = role, Id = Guid.NewGuid() });
                    }
                }

                throw new Exception("TODO: Add an Enabled field to the User Class/Entity, or remove/modify the next line");
                AddUser(userManager, roleManager, "your@email.address", "YourFirstName", "YourSurname", "YourPassword", allRoles);

                if (!context.Settings.Any())
                {
                    var settings = new Settings();
                    context.Entry(settings).State = EntityState.Added;
                    context.SaveChanges();
                }
            }
        }

        private void AddUser(AppUserManager userManager, RoleManager<AppRole, Guid> roleManager, string email, string firstName, string lastName, string password, List<string> roles)
        {
            throw new Exception("TODO: Add these fields to the User Class/Entity, or remove/modify the next line");
            User user;
            if (userManager.FindByEmail(email) == null)
            {
                user = new User
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                    //FirstName = firstName,
                    //LastName = lastName,
                    //Enabled = true
                };
                userManager.Create(user, password);
            }

            user = userManager.FindByEmail(email);

            foreach (var role in roles)
            {
                if (!userManager.IsInRole(user.Id, role))
                {
                    userManager.AddToRole(user.Id, role);
                }
            }
        }
    }
}
