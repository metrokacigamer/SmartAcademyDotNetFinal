using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;

namespace Persistence.Repositories
{
	public class ProductRepository : IRepository<Product>
	{
		private DbSet<Product> _products;
		private readonly ShoppingAppDbContext _context;

		public ProductRepository(ShoppingAppDbContext context)
        {
            _context = context;
			_products = context.Set<Product>();
        }

        public void Create(Product entity)
		{
			_products.Add(entity);
			_context.SaveChanges();
		}

		public void Delete(Product entity)
		{
			_products.Remove(entity);
			_context.SaveChanges();
		}

		public async Task<IEnumerable<Product>> GetAllAsync()
		{
			return await Task.FromResult(_products);
		}

		public async Task<Product> GetByIdAsync(string id)
		{
			return await _products.FirstAsync(x => x.Id == id);
		}

		public void Update(Product entity)
		{
			_products.Update(entity);
			_context.SaveChanges();
		}
	}
}
