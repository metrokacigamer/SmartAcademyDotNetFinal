using Service.Abstactions;
using Domain.Repositories;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Services;

namespace services
{
	public class ServiceManager : IServiceManager
	{
		private readonly IAccountService _accountService;
		private readonly IProductService _productService;
		private readonly IImageService _imageService;
		private readonly ICartService _cartService;
		private readonly IItemService _itemService;
		private readonly IEmailSenderService _emailSenderService;

		public ServiceManager(IRepositoryManager repositoryManager,
								UserManager<AppUser> userManager,
								RoleManager<IdentityRole> roleManager,
								SignInManager<AppUser> signInManager,
								IHttpContextAccessor httpContextAccessor)
		{
			_accountService = new AccountService(repositoryManager, userManager, roleManager, signInManager);
			_productService = new ProductService(repositoryManager);
			_imageService = new ImageService(repositoryManager);
			_cartService = new CartService(repositoryManager, httpContextAccessor);
			_itemService = new ItemService(repositoryManager);
			_emailSenderService = new EmailSenderService(httpContextAccessor, repositoryManager);
		}

		public IAccountService AccountService => _accountService;

		public IProductService ProductService => _productService;

		public IImageService ImageService => _imageService;

		public ICartService CartService => _cartService;

		public IItemService ItemService => _itemService;

		public IEmailSenderService EmailSenderService => _emailSenderService;

	}
}
