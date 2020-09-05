using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CampusLocation.Data
{
    public class SeedData
    {
        public async static Task Seed(UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            await SeedRoles(roleManager);
            await SeedUsers(userManager);
        }
        private async static Task SeedUsers(UserManager<IdentityUser> userManager)
        {
            if (await userManager.FindByEmailAsync("admin@email.com") == null)
            {
                var user = new IdentityUser
                {
                    UserName = "admin@email.com",
                    Email = "admin@email.com"
                };
                var result = await userManager.CreateAsync(user, "_Password01");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Administrator");
                }
            }
            if (await userManager.FindByEmailAsync("student@email.com") == null)
            {
                var user = new IdentityUser
                {
                    UserName = "student@email.com",
                    Email = "student@email.com"
                };
                var result = await userManager.CreateAsync(user, "_Password02");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Student");
                }
            }
            
        }

        private async static Task SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            if (!await roleManager.RoleExistsAsync("Administrator"))
            {
                var role = new IdentityRole
                {
                    Name = "Administrator"
                };
                await roleManager.CreateAsync(role);
            }

            if (!await roleManager.RoleExistsAsync("Student"))
            {
                var role = new IdentityRole
                {
                    Name = "Student"
                };
                await roleManager.CreateAsync(role);
            }
        }
    }
}
