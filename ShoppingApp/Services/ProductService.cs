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

S		public async Task<Product> AddProduct(AddProductViewModel model)//tested
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
				Category = model.Category,
				Description = model.Description,
				QuantityInStock = model.QuantityInStock,
			};
			_repositoryManager.ProductRepository.Create(product);

			return product;
		}

		public void AdjustQuantity(IEnumerable<Item> items)//tested
		{
			foreach (var item in items)
			{
				var product = item.Product;
				product.QuantityInStock -= product.QuantityInStock > item.Quantity ? item.Quantity : product.QuantityInStock;
				_repositoryManager.ProductRepository.Update(product);
			}
		}

		public async Task<Product> GetById(string id)//tested
		{
			return await _repositoryManager.ProductRepository.GetByIdAsync(id);
		}

		public async Task<IEnumerable<ProductViewModel>> GetProductsViewModels(string searchString, int currentPage, int pageSize)//tested
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

		private ProductViewModel MapToProductViewModel(Product product)//tested
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

		public async Task<EditProductViewModel> GetEditProductViewModel(string productId)//tested
		{
			var product = await _repositoryManager.ProductRepository.GetByIdAsync(productId);
			return new EditProductViewModel
			{
				Id = product.Id,
				Name = product.Name,
				Description = product.Description,
				QuantityInStock = product.QuantityInStock,
				Category = product.Category,
				Price = product.Price,
			};
		}


		public async Task<FilteredPageViewModel> Filter(FilterViewModel model, int currentPage, int pageSize)//tested
		{
			var products = (await _repositoryManager.ProductRepository.GetAllAsync());
			var filterProducts = products.Where(product =>
			{
				bool priceFilter = PriceFilter(model, product);
				bool searchStringFilter = SearchStringFilter(model, product);
				bool categoryFilter = CategoryFilter(model, product);

				return priceFilter && searchStringFilter && categoryFilter;
			})
			.Skip(currentPage * pageSize)
			.Take(pageSize);
			var productVMs = filterProducts.Select(x => MapToProductViewModel(x));

			return new FilteredPageViewModel
			{
				ProductViewModels = productVMs,
				Filter = model,
			};

		}

		private bool CategoryFilter(FilterViewModel model, Product product)//tested
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

		private bool PriceFilter(FilterViewModel model, Product product)//tested
		{
			if (model.PriceUpperBound != default && model.PriceLowerBound < model.PriceUpperBound)
			{
				return product.Price >= model.PriceLowerBound &&
						product.Price <= model.PriceUpperBound;
			}
			else
			{
				return true;
			}
		}

		private bool SearchStringFilter(FilterViewModel model, Product product)//tested
		{
			if (model.SearchString != default)
			{
				return product.Name.Contains(model.SearchString) ||
					product.Description.Contains(model.SearchString) ||
					product.Category.ToString().Contains(model.SearchString);
			}
			else
			{
				return true;
			}
		}

		public async Task UpdateProduct(EditProductViewModel model)//tested
		{
			var product = await _repositoryManager.ProductRepository.GetByIdAsync(model.Id);
			product.Description = model.Description;
			product.Name = model.Name;
			product.Price = model.Price;
			product.Category = model.Category;
			product.QuantityInStock = model.QuantityInStock;
			_repositoryManager.ProductRepository.Update(product);
		}

		public async Task<ProductViewModel> GetProductViewModel(string productId)//tested
		{
			var product = await _repositoryManager.ProductRepository.GetByIdAsync(productId);

			return MapToProductViewModel(product);
		}

		public async Task DeleteProduct(string productId)//tested
		{
			var product = await _repositoryManager.ProductRepository.GetByIdAsync(productId);
			_repositoryManager.ProductRepository.Delete(product);
		}

		public async Task<int> GetTotalPages(FilterViewModel filter, int pageSize)//tested
		{
			var products = await _repositoryManager.ProductRepository.GetAllAsync();
			var models = products.Where(product =>
			{
				bool priceFilter = PriceFilter(filter, product);
				bool searchStringFilter = SearchStringFilter(filter, product);
				bool categoryFilter = CategoryFilter(filter, product);

				return priceFilter && searchStringFilter && categoryFilter;
			});
			return await Task.FromResult(models.Count()/pageSize + 1);
		}

		public async Task<IEnumerable<ProductViewModel>> SortBy(IEnumerable<ProductViewModel> productVMs,
																string sortBy,
																bool ascending)//tested
		{
			switch(sortBy)
			{
				case "Price":
					{
						if (ascending)
						{
							productVMs = productVMs.OrderBy(x => x.Price);
						}
						else
						{
							productVMs = productVMs.OrderByDescending(x => x.Price);
						}

						break;
					}
				case "Name":
					{
						if (ascending)
						{
							productVMs = productVMs.OrderBy(x => x.Name);
						}
						else
						{
							productVMs = productVMs.OrderByDescending(x => x.Name);
						}

						break;
					}
				case "Category":
					{
						if (ascending)
						{
							productVMs = productVMs.OrderBy(x => x.Category);
						}
						else
						{
							productVMs = productVMs.OrderByDescending(x => x.Category);
						}

						break;
					}
				default:
					{
						goto case "Price";
					}
			}
			return await Task.FromResult(productVMs);
		}
	}
}
