using HotelBookingSystem.Models;
using Microsoft.AspNetCore.Identity;

namespace HotelBookingSystem.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // Seed Roles
            string[] roles = { "Admin", "Receptionist", "Manager" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // Seed Admin User
            var adminEmail = "admin@hotel.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "System Administrator",
                    EmailConfirmed = true,
                    IsActive = true,
                    CreatedBy = "System"
                };
                var result = await userManager.CreateAsync(adminUser, "Admin@123");
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(adminUser, "Admin");
            }

            // Seed Receptionist User
            var recepEmail = "receptionist@hotel.com";
            var recepUser = await userManager.FindByEmailAsync(recepEmail);
            if (recepUser == null)
            {
                recepUser = new ApplicationUser
                {
                    UserName = recepEmail,
                    Email = recepEmail,
                    FullName = "Front Desk Receptionist",
                    EmailConfirmed = true,
                    IsActive = true,
                    CreatedBy = adminEmail
                };
                var result = await userManager.CreateAsync(recepUser, "Recep@123");
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(recepUser, "Receptionist");
            }
        }
    }
}
