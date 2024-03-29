using Microsoft.AspNetCore.Identity;

namespace ShoppingApp
{
	public class Seed
	{
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
