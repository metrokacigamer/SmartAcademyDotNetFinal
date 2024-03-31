using Domain.Repositories;
using Domain.Entities;

namespace Persistence.Repositories
{
	public class RepositoryManager : IRepositoryManager
	{
		private readonly IRepository<Product> _productRepository;
		private readonly IRepository<Item> _itemRepository;
		private readonly IRepository<Cart> _cartRepository;
		private readonly IRepository<Image> _imageRepository;

		public RepositoryManager(ShoppingAppDbContext context)
        {
            _productRepository = new ProductRepository(context);
        }

		public IRepository<Cart> CartRepository => _cartRepository;

		public IRepository<Image> ImageRepository => _imageRepository;

		public IRepository<Item> ItemRepository => _itemRepository;

		public IRepository<Product> ProductRepository => _productRepository;
	}
}
