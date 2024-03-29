using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;


namespace Persistence.Repositories
{
	public class ItemRepository : IRepository<Item>
	{
		private DbSet<Item> _items;
		private readonly ShoppingAppDbContext _context;

		public ItemRepository(ShoppingAppDbContext context)
		{
			_context = context;
			_items = context.Set<Item>();
		}

		public void Create(Item entity)
		{
			_items.Add(entity);
		}

		public void Delete(Item entity)
		{
			_items.Remove(entity);
		}

		public async Task<IEnumerable<Item>> GetAllAsync()
		{
			return await Task.FromResult(_items);
		}

		public async Task<Item> GetByIdAsync(string id)
		{
			return await _items.FirstAsync(x => x.Id == id);
		}

		public void Update(Item entity)
		{
			_items.Update(entity);
		}
	}
}
