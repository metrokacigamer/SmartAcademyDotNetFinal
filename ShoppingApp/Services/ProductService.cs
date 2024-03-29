using Domain.Exceptions;
using Domain.Repositories;
using Domain.Entities;
using Service.Abstactions;
using Shared.Models;
using Domain.Enums;

namespace Services
{
	internal class ProductService : IProductService
	{
		private readonly IRepositoryManager _repositoryManager;

		public ProductService(IRepositoryManager repositoryManager)
        {
			_repositoryManager = repositoryManager;
		}

        public async Task<Product> AddProduct(AddProductViewModel model)
		{
			var productExists = (await _repositoryManager.ProductRepository().GetAllAsync()).Any(x => x.Name == model.Name);
			if (productExists)
			{
				throw new ProductExistsException(); //not the best practice imo
			}
			var product = new Product
			{
				Name = model.Name,
				Price = model.Price,
				Catregory = (Category)Enum.Parse(typeof(Category), model.Category, false),
				Description = model.Description,
			};
			_repositoryManager.ProductRepository().Create(product);

			return product;
			//foreach (var imageFile in model.ProductImages)
			//{
			//	var fileExtension = Path.GetExtension(imageFile.FileName).ToLower();

			//	if (imageFile != null && imageFile.Length > 0 && !(fileExtension != ".jpg" && fileExtension != ".jpeg" && fileExtension != ".png"))
			//	{
			//		var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
			//		var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", fileName);
			//		var relativePath = $@"~/images/{fileName}";

			//		using (var stream = new FileStream(imagePath, FileMode.Create))
			//		{
			//			await imageFile.CopyToAsync(stream);
			//		}

			//		var image = new Image
			//		{
			//			Product = product,
			//			ImagePath = relativePath,
			//		};

			//		_repositoryManager.ImageRepository().Create(image);
			//	}
			//}
		}
	}
}
