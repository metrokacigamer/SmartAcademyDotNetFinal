using Service.Abstactions;
using Domain.Repositories;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Domain.Wrappers;

namespace Services
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
								ISessionWrapper sessionWrapper,
								ISmtpClientWrapper smtpClientWrapper, 
								IFileStreamWrapper formFileWrapper
								)
		{
			_accountService = new AccountService(repositoryManager, userManager, roleManager, signInManager);
			_productService = new ProductService(repositoryManager);
			_imageService = new ImageService(repositoryManager, formFileWrapper);
			_cartService = new CartService(repositoryManager, sessionWrapper);
			_itemService = new ItemService(repositoryManager);
			_emailSenderService = new EmailSenderService(sessionWrapper, repositoryManager, smtpClientWrapper);
		}

		public IAccountService AccountService => _accountService;

		public IProductService ProductService => _productService;

		public IImageService ImageService => _imageService;

		public ICartService CartService => _cartService;

		public IItemService ItemService => _itemService;

		public IEmailSenderService EmailSenderService => _emailSenderService;

	}
}
