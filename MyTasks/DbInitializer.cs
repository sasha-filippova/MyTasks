using Microsoft.AspNetCore.Identity;
using MyTasks.Models;
using System;
using System.Threading.Tasks;

namespace MyTasks
{
    public static class DbInitializer
    {
        public static async System.Threading.Tasks.Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Ensure roles exist
            string[] roles = { "Admin", "Manager", "Employee" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    var roleResult = await roleManager.CreateAsync(new IdentityRole(role));
                    if (!roleResult.Succeeded)
                    {
                        throw new Exception($"Failed to create role {role}: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
                    }
                }
            }

            // Add admin user
            var admin = new ApplicationUser
            {
                UserName = "admin",
                Email = "admin@example.com",
                Name = "Алексей",
                Surname = "Иванов",
                Patronymic = "Сергеевич",
                Role = "Admin"
            };

            await EnsureUserAsync(userManager, admin, "AdminPassword123!", "Admin");

            // Add manager user
            var manager = new ApplicationUser
            {
                UserName = "manager",
                Email = "manager@example.com",
                Name = "Олег",
                Surname = "Петров",
                Patronymic = "Алексеевич",
                Role = "Manager"
            };

            await EnsureUserAsync(userManager, manager, "ManagerPassword123!", "Manager");

            // Add or update employee users with unique passwords
            var employees = new[]
            {
            new { User = new ApplicationUser { UserName = "employee1", Email = "employee1@example.com", Name = "Ольга", Surname = "Сидорова", Patronymic = "Ивановна", Role = "Employee" }, Password = "Password1!" },
            new { User = new ApplicationUser { UserName = "employee2", Email = "employee2@example.com", Name = "Александра", Surname = "Петрова", Patronymic = "Александровна", Role = "Employee" }, Password = "Password2!" },
            new { User = new ApplicationUser { UserName = "employee3", Email = "employee3@example.com", Name = "Максим", Surname = "Кузнецов", Patronymic = "Михайлович", Role = "Employee" }, Password = "Password3!" }
        };

            foreach (var employee in employees)
            {
                await EnsureUserAsync(userManager, employee.User, employee.Password, "Employee");
            }
        }

        private static async System.Threading.Tasks.Task EnsureUserAsync(UserManager<ApplicationUser> userManager, ApplicationUser user, string password, string role)
        {
            var existingUser = await userManager.FindByNameAsync(user.UserName);
            if (existingUser == null)
            {
                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    var roleResult = await userManager.AddToRoleAsync(user, role);
                    if (!roleResult.Succeeded)
                    {
                        throw new Exception($"Failed to add user {user.UserName} to role {role}: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
                    }
                }
                else
                {
                    throw new Exception($"Failed to create user {user.UserName}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }
            else
            {
                existingUser.Email = user.Email;
                existingUser.Name = user.Name;
                existingUser.Surname = user.Surname;
                existingUser.Patronymic = user.Patronymic;
                existingUser.Role = user.Role;
                var updateResult = await userManager.UpdateAsync(existingUser);

                if (!updateResult.Succeeded)
                {
                    throw new Exception($"Failed to update user {user.UserName}: {string.Join(", ", updateResult.Errors.Select(e => e.Description))}");
                }
                // Update password if necessary
                if (!await userManager.CheckPasswordAsync(existingUser, password))
                {
                    var removePasswordResult = await userManager.RemovePasswordAsync(existingUser);
                    if (!removePasswordResult.Succeeded)
                    {
                        throw new Exception($"Failed to remove password for user {existingUser.UserName}: {string.Join(", ", removePasswordResult.Errors.Select(e => e.Description))}");
                    }

                    var addPasswordResult = await userManager.AddPasswordAsync(existingUser, password);
                    if (!addPasswordResult.Succeeded)
                    {
                        throw new Exception($"Failed to add password for user {existingUser.UserName}: {string.Join(", ", addPasswordResult.Errors.Select(e => e.Description))}");
                    }
                }

                // Ensure user is in the correct role
                var userRoles = await userManager.GetRolesAsync(existingUser);
                if (!userRoles.Contains(role))
                {
                    var addToRoleResult = await userManager.AddToRoleAsync(existingUser, role);
                    if (!addToRoleResult.Succeeded)
                    {
                        throw new Exception($"Failed to add user {existingUser.UserName} to role {role}: {string.Join(", ", addToRoleResult.Errors.Select(e => e.Description))}");
                    }
                }
            }
        }
    }
}
