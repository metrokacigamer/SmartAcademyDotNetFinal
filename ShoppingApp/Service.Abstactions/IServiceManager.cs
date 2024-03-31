namespace Service.Abstactions
{
	public interface IServiceManager
	{
		IAccountService AccountService { get; }
		IProductService ProductService { get; }
		IImageService ImageService { get; }
		ICartService CartService { get; }
		IItemService ItemService { get; }
		IEmailSenderService EmailSenderService { get; }
	}
}
