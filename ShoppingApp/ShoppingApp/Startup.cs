using Domain.Entities;
using Domain.Repositories;
using Domain.Wrappers;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;
using Persistence;
using Persistence.Repositories;
using Presentation;
using Service.Abstactions;
using Services;
using Persistence.Wrappers;

namespace ShoppingApp
{
	public static class Startup
	{
		public static void ConfigureServices(this IServiceCollection services)
		{
			services.AddSession(options =>
			{
				options.Cookie.Name = "Guest.Session";
				options.IdleTimeout = TimeSpan.FromMinutes(20);
				options.Cookie.IsEssential = true;
			});
			services.AddIdentity<AppUser, IdentityRole>(options =>
			{
				options.Password.RequiredLength = 8;
				options.Password.RequireNonAlphanumeric = false;
				options.Password.RequireLowercase = false;
				options.Password.RequireUppercase = false;
			})
			.AddEntityFrameworkStores<ShoppingAppDbContext>()
			.AddDefaultTokenProviders();
			services.AddHttpContextAccessor();

			services.AddScoped<IRepositoryManager, RepositoryManager>();
			services.AddScoped<ISmtpClientWrapper, SmtpClientWrapper>();
			services.AddScoped<IFileStreamWrapper, FileStreamWrapper>();

			services.AddMvc(x =>
			{
				x.Filters.Add(typeof(ModelStateFeatureFilter));
			});
			services.AddTransient<ExceptionHandlingMiddleware>();
			services.AddScoped<ISessionWrapper, SessionWrapper>();
			services.AddScoped<IServiceManager, ServiceManager>(); 
		}
	}
}
