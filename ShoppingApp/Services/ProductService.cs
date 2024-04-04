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
			var productExists = (await _repositoryManager.ProductRepository.GetAllAsync()).Any(x => x.Name == model.Name);
			if (productExists)
			{
				throw new ProductExistsException(); //not the best practice imo
			}
			var product = new Product
			{
				Name = model.Name,
				Price = model.Price,
				Category = (Category)Enum.Parse(typeof(Category), model.Category, false),
				Description = model.Description,
			};
			_repositoryManager.ProductRepository.Create(product);

			return product;
		}

		public void AdjustQuantity(IEnumerable<Item> items)
		{
			foreach (var item in items)
			{
				var product = item.Product;
				product.QuantityInStock -= product.QuantityInStock > item.Quantity ? item.Quantity : product.QuantityInStock;
				_repositoryManager.ProductRepository.Update(product);
			}
		}

		public async Task<Product> GetById(string id)
		{
			return await _repositoryManager.ProductRepository.GetByIdAsync(id);
		}

		public async Task<IEnumerable<ProductViewModel>> GetProductsViewModels(string searchString, int currentPage, int pageSize)
		{
			var products = (await _repositoryManager.ProductRepository.GetAllAsync())
												.Where(x =>
												{
													return x.Name.Contains(searchString) ||
													x.Description.Contains(searchString) ||
													x.Category.ToString().Contains(searchString);
												})
												.Skip(currentPage * pageSize)
												.Take(pageSize);
			var productVMs = new List<ProductViewModel>();
			foreach (var product in products)
			{
				productVMs.Add(MapToProductViewModel(product));
			}

			return productVMs;
		}

		private ProductViewModel MapToProductViewModel(Product product)
		{
			return new ProductViewModel
			{
				Id = product.Id,
				Name = product.Name,
				Description = product.Description,
				QuantityInStock = product.QuantityInStock,
				Category = product.Category,
				Price = product.Price,
			};
		}

		public async Task<EditProductViewModel> GetEditProductViewModel(string productId)
		{
			var product = await _repositoryManager.ProductRepository.GetByIdAsync(productId);
			return new EditProductViewModel
			{
				Id = product.Id,
				Name= product.Name,
				Description = product.Description,
				QuantityInStock = product.QuantityInStock,
				Category = product.Category,
				Price = product.Price,
			};
		}


		public async Task<FilteredPageViewModel> Filter(FilterViewModel model)
		{
			var products = (await _repositoryManager.ProductRepository.GetAllAsync());
			var filterProducts = products.Where(product =>
			{
				bool priceFilter = PriceFilter(model, product);
				bool searchStringFilter = SearchStringFilter(model, product);
				bool categoryFilter = CategoryFilter(model, product);

				return priceFilter && searchStringFilter && categoryFilter;
			});
			var productVMs = filterProducts.Select(x => MapToProductViewModel(x));

			return new FilteredPageViewModel
			{
				ProductViewModels = productVMs,
				Filter = model,
			};

		}

		private bool CategoryFilter(FilterViewModel model, Product product)
		{
			if (model.Category != 0)
			{
				return product.Category == model.Category;
			}
			else
			{
				return true;
			}
		}

		private bool PriceFilter(FilterViewModel model, Product product)
		{
			if (model.PriceLowerBound != default && model.PriceUpperBound != default && model.PriceLowerBound < model.PriceUpperBound)
			{
				return product.Price >= model.PriceLowerBound && 
						product.Price <= model.PriceUpperBound;
			}
			else
			{
				return true;
			}
		}

		private bool SearchStringFilter(FilterViewModel model, Product product)
		{
			if (model.SearchString != default)
			{
				return product.Name.Contains(model.SearchString) ||
					product.Description.Contains(model.SearchString);
			}
			else
			{
				return true;
			}
		}

		public async Task UpdateProduct(EditProductViewModel model)
		{
			var product = await _repositoryManager.ProductRepository.GetByIdAsync(model.Id);
			product.Description = model.Description;
			product.Name = model.Name;
			product.Price = model.Price;
			product.Category = model.Category;
			product.QuantityInStock = model.QuantityInStock;
			_repositoryManager.ProductRepository.Update(product);
		}

		public async Task<ProductViewModel> GetProductViewModel(string productId)
		{
			var product = await _repositoryManager.ProductRepository.GetByIdAsync(productId);

			return MapToProductViewModel(product);
		}

		public async Task DeleteProduct(string productId)
		{
			var product = await _repositoryManager.ProductRepository.GetByIdAsync(productId);
			_repositoryManager.ProductRepository.Delete(product);
		}
	}
}
