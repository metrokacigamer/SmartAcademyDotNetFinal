using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace ShoppingApp
{
	public class Seed
	{
		public static async Task SeedAdminAsync(IApplicationBuilder applicationBuilder)
		{
			using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
			{
				//var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
				var userManager = serviceScope.ServiceProvider.GetService<UserManager<AppUser>>();
				if (!userManager!.Users.Any())
				{
					var signInManager = serviceScope.ServiceProvider.GetService<SignInManager<AppUser>>();
					var user = new AppUser()
					{
						Email = "urmail@gmail.com",
						UserName = "metrokacigamer",
					};

					var result = await userManager.CreateAsync(user, "@Adminpass1");
					if (result.Succeeded)
					{
						await userManager.AddToRoleAsync(user, Roles.Admin);
					}
				}
			}
		}
		public static async Task SeedRolesAsync(IApplicationBuilder applicationBuilder)
		{
			using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
			{
				var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

				if (!await roleManager.RoleExistsAsync(Roles.Admin))
					await roleManager.CreateAsync(new IdentityRole(Roles.Admin));
				if (!await roleManager.RoleExistsAsync(Roles.Guest))
					await roleManager.CreateAsync(new IdentityRole(Roles.Guest));
			}
		}
	}
}
