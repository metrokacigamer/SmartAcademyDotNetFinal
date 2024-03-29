using Domain.Entities;

namespace Domain.Repositories
{
	public interface IRepositoryManager
	{
		IRepository<Product> ProductRepository();
		IRepository<Item> ItemRepository();
		IRepository<Cart> CartRepository();
		IRepository<Image> ImageRepository();
	}
}
