using backend.DataContext;
using backend.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Helpers
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(IServiceProvider _serviceProvider)
        {
            var roleManager = _serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = _serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var context = _serviceProvider.GetRequiredService<ApplicationDbContext>();


            // Seed Roles
            string[] roles = { "Admin", "User" };
            foreach(var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // Seed Admin
            string adminEmail = "admin123@gmail.com";
            string adminPassword = "Admin@123";

            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if(adminUser == null)
            {
                var newAdmin = new ApplicationUser
                {
                    UserName = "admin123",
                    Email = adminEmail,
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(newAdmin, adminPassword);
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(newAdmin, "Admin");
            }

            // Seeding Account Types
            if (!context.AccountTypes.Any())
            {
                var accountTypes = new List<AccountType>
                {
                    new AccountType { Id = Guid.NewGuid(), Type = "Indivdual", MaxAccounts = 1},
                    new AccountType { Id = Guid.NewGuid(), Type = "Duo", MaxAccounts = 2}
                };

                context.AccountTypes.AddRange(accountTypes);
                await context.SaveChangesAsync();
            }

            //Seeding Categories    
            if (!context.Categories.Any())
            {
                var categories = new List<Category>
                {
                    new Category { Id = Guid.NewGuid(), Name = "Housing"},
                    new Category { Id = Guid.NewGuid(), Name = "Utilities"},
                    new Category { Id = Guid.NewGuid(), Name = "Transportation"},
                    new Category { Id = Guid.NewGuid(), Name = "Food and Groceries"},
                    new Category { Id = Guid.NewGuid(), Name = "Health & Medical"},
                    new Category { Id = Guid.NewGuid(), Name = "Insurance"},
                    new Category { Id = Guid.NewGuid(), Name = "Debt Payments"},
                    new Category { Id = Guid.NewGuid(), Name = "Savings & Investments"},
                    new Category { Id = Guid.NewGuid(), Name = "Entertainment"},
                    new Category { Id = Guid.NewGuid(), Name = "Travel"},
                    new Category { Id = Guid.NewGuid(), Name = "Shopping"},
                    new Category { Id = Guid.NewGuid(), Name = "Personal Care"},
                    new Category { Id = Guid.NewGuid(), Name = "Subscriptions"},
                    new Category { Id = Guid.NewGuid(), Name = "Miscellaneous"},
                    new Category { Id = Guid.NewGuid(), Name = "Salary"},
                    new Category { Id = Guid.NewGuid(), Name = "Freelance"},
                    new Category { Id = Guid.NewGuid(), Name = "Investment Income"},
                    new Category { Id = Guid.NewGuid(), Name = "Business Income"},
                    new Category { Id = Guid.NewGuid(), Name = "Gifts/Other Income"},
                };

                context.Categories.AddRange(categories);
                await context.SaveChangesAsync();
            }
        }
    }
}
