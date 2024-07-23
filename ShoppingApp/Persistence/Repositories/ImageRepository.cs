using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;


namespace Persistence.Repositories
{
	public class ImageRepository : IRepository<Image>
	{
		private DbSet<Image> _images;
		private readonly ShoppingAppDbContext _context;

		public ImageRepository(ShoppingAppDbContext context)
		{
			_context = context;
			_images = context.Set<Image>();
		}

		public void Create(Image entity)
		{
			_images.Add(entity);
			_context.SaveChanges();
		}

		public void Delete(Image entity)
		{
			_images.Remove(entity);
			_context.SaveChanges();
		}

		public async Task<IEnumerable<Image>> GetAllAsync()
		{
			return await Task.FromResult(_images);
		}

		public async Task<Image> GetByIdAsync(string id)
		{
			return await _images.FirstAsync(x => x.Id == id);
		}

		public void Update(Image entity)
		{
			_images.Update(entity);
			_context.SaveChanges();
		}
	}
}

