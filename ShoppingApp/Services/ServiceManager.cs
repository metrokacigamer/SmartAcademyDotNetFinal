using Service.Abstactions;
using Domain.Repositories;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Services
{
	public class ServiceManager: IServiceManager
	{
		private readonly IAccountService _accountService;
		private readonly IProductService _productService;
		private readonly IImageService _imageService;

		public ServiceManager(IRepositoryManager repositoryManager,
								UserManager<AppUser> userManager,
								RoleManager<IdentityRole> roleManager,
								SignInManager<AppUser> signInManager)
		{
			_accountService = new AccountService(repositoryManager, userManager, roleManager, signInManager);
			_productService = new ProductService(repositoryManager);
			_imageService = new ImageService(repositoryManager);
		}

		public IAccountService AccountService => _accountService;

		public IProductService ProductService => _productService;
		public IImageService ImageService => _imageService;

	}
}
