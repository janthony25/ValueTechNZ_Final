using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SQLitePCL;
using ValueTechNZ_Final.Models;

namespace ValueTechNZ_Final.Services
{
    public class SeedData
    {
        
        public static async Task DatabaseInitializer(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // Ensuring that roles exists
            if (!await roleManager.RoleExistsAsync("Admin"))
                await roleManager.CreateAsync(new IdentityRole("Admin"));

            if (!await roleManager.RoleExistsAsync("Client"))
                await roleManager.CreateAsync(new IdentityRole("Client"));


            // Ensure that admin user exists
            var adminUser = await userManager.GetUsersInRoleAsync("Admin");
            if(adminUser.Count == 0)
            {
                var admin = new ApplicationUser
                {
                    UserName = "admin@gmail.com",
                    Email = "admin@gmail.com",
                    NormalizedUserName = "admin@gmail.com",
                    NormalizedEmail = "admin@gmail.com",
                    FirstName = "admin",
                    LastName = "user",
                    Address = "Admin Address",
                    CreatedAt = DateTime.Now,
                    EmailConfirmed = true
                };

                string adminPassword = "J@nth0ny25";
                var result = await userManager.CreateAsync(admin, adminPassword);
                if (result.Succeeded)
                {
                    // Add the new user to ADMIN ROLE
                    await userManager.AddToRoleAsync(admin, "Admin");
                }
                else
                {
                    foreach(var error in result.Errors)
                    {
                        Console.WriteLine($"Unable to create user. Errors: {error.Description}");
                    }
                }
            }
        }
    }
}
