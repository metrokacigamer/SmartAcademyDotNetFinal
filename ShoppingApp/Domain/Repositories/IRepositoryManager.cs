using Domain.Entities;

namespace Domain.Repositories
{
	public interface IRepositoryManager
	{
		IRepository<Product> ProductRepository { get; }
		IRepository<Item> ItemRepository { get; }
		IRepository<Cart> CartRepository { get; }
		IRepository<Image> ImageRepository { get; }
	}
}
