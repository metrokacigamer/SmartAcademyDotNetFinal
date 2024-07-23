using Domain.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;


namespace Persistence.Repositories
{
	public class CartRepository: IRepository<Cart>
	{
		private DbSet<Cart> _carts;
		private readonly ShoppingAppDbContext _context;

		public CartRepository(ShoppingAppDbContext context)
		{
			_context = context;
			_carts = context.Set<Cart>();
		}

		public void Create(Cart entity)
		{
			_carts.Add(entity);
			_context.SaveChanges();
		}

		public void Delete(Cart entity)
		{
			_carts.Remove(entity);
			_context.SaveChanges();
		}

		public async Task<IEnumerable<Cart>> GetAllAsync()
		{
			return await Task.FromResult(_carts);
		}

		public async Task<Cart> GetByIdAsync(string id)
		{
			return await _carts.FirstAsync(x => x.Id == id);
		}

		public void Update(Cart entity)
		{
			_carts.Update(entity);
			_context.SaveChanges();
		}
	}
}
