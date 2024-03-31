using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Service.Abstactions;
using Domain.Entities;
using services;
using Domain.Repositories;
using Persistence.Repositories;
using Persistence;
using Presentation;

namespace ShoppingApp
{
    public class Program
	{
		public static async Task Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.
			builder.Services.AddControllersWithViews().AddApplicationPart(typeof(AssemblyReference).Assembly);
			builder.Services.AddDbContext<ShoppingAppDbContext>(x => {
				x.UseSqlServer(builder.Configuration.GetConnectionString("ConString"))
				.UseLazyLoadingProxies();
				});

			builder.Services.ConfigureServices();

			var app = builder.Build();

			await Seed.SeedRolesAsync(app);
			await Seed.SeedAdminAsync(app);

			// Configure the HTTP request pipeline.
			if (!app.Environment.IsDevelopment())
			{
				app.UseExceptionHandler("/Home/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseMiddleware<ExceptionHandlingMiddleware>();
			app.UseSession();

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();
			
			app.UseAuthentication();
			app.UseAuthorization();

			app.MapControllerRoute(
				name: "default",
				pattern: "{controller=Home}/{action=Index}/{id?}");

			app.Run();
		}
	}
}
