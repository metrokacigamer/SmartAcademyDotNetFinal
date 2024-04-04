using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using Domain.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using Persistence;
using Shared.Models;
using System.Text.Json;


namespace Services.Test
{
	public class ProductServiceTests
	{
		private ProductService _productService;
		private Mock<IRepositoryManager> _repositoryManagerMock;
		private Mock<IRepository<Product>> _productRepositoryMock;


		[SetUp]
		public void Setup()
		{
			_repositoryManagerMock = new Mock<IRepositoryManager>();
			_productService = new ProductService(_repositoryManagerMock.Object);
			_productRepositoryMock = new Mock<IRepository<Product>>();
			_repositoryManagerMock.Setup(x => x.ProductRepository).Returns(_productRepositoryMock.Object);
		}

		[Test]
		public async Task AddProduct_ProductDoesntExist_AddsProduct()
		{
			//Arrange
			var model = new AddProductViewModel
			{
				Category = Category.Other.ToString(),
				Description = "testDescription",
				Name = "Test",
				Price = 10.0,
				ProductImages = new List<IFormFile>(),
			};
			var product = new Product
			{
				Category = (Category)Enum.Parse(typeof(Category), model.Category),
				Name = model.Name,
				Price = model.Price,
				Description = model.Description,
			};

			_repositoryManagerMock.Setup(x => x.ProductRepository.GetAllAsync()).ReturnsAsync(new List<Product>());

			//Act
			var result = await _productService.AddProduct(model);
			var resultJson = JsonSerializer.Serialize(result);
			var expectedJson = JsonSerializer.Serialize(product);

			//Assert
			Assert.NotNull(result);
			Assert.AreEqual(expectedJson, resultJson);
		}

		[Test]
		public async Task AddProduct_ProductExists_ThrowsProductExistsException()
		{
			//Arrange
			var model = new AddProductViewModel
			{
				Name = "Test",
			};
			var product = new Product
			{
				Name = model.Name,
			};

			_repositoryManagerMock.Setup(x => x.ProductRepository.GetAllAsync()).ReturnsAsync(new List<Product>() { product });

			//Act & Assert
			Assert.ThrowsAsync<ProductExistsException>(async () => await _productService.AddProduct(model));
		}

		[Test]
		public async Task AdjustQuantity_test_works()
		{
			//Arrange
			var product1 = new Product { QuantityInStock = 10 };
			var product2 = new Product { QuantityInStock = 8 };

			var items = new List<Item>()
			{
				new Item { Product = product1, Quantity = 5 },
				new Item { Product = product2, Quantity = 3 },
				new Item { Product = product2, Quantity = 8 }
			};

			// Act
			_productService.AdjustQuantity(items);

			// Assert
			_repositoryManagerMock.Verify(repo => repo.ProductRepository.Update(It.IsAny<Product>()), Times.Exactly(3));
			Assert.AreEqual(5, items[0].Product.QuantityInStock);
			Assert.AreEqual(0, items[1].Product.QuantityInStock);
		}

		[Test]
		public async Task GetById_NoSuchId_ReturnsNull()
		{
			//Arrange
			var testId1 = "";

			//Act
			var result = await _productService.GetById(testId1);

			//Assert
			Assert.IsNull(result);
		}

		[Test]
		public async Task GetProductsViewModels_NoSuchId_ReturnsNull()
		{
			//Arrange
			var list = new List<Product>()
			{
				new Product
				{
					Name = "est",
					Description = "sss",
					Category = Category.Other,
				},
				new Product
				{
					Name = "es",
					Description = "23test",
				},
				new Product
				{
					Name = "sdaest",
					Description = "sss",
					Category = Category.Other,
				},
			};
			var searchString = "Other";
			var currentPage = 0;
			var pagesize = 3;
			_repositoryManagerMock.Setup(x => x.ProductRepository.GetAllAsync()).ReturnsAsync(list);
			var expected = new List<ProductViewModel>()
			{
				new ProductViewModel
				{
					Name = "est",
					Description = "sss",
					Category = Category.Other,              },
				new ProductViewModel
				{
					Name = "sdaest",
					Description = "sss",
					Category = Category.Other,
				},
			};

			//Act
			var result = await _productService.GetProductsViewModels(searchString, currentPage, pagesize);

			var expectedJson = JsonSerializer.Serialize(expected);
			var resultJson = JsonSerializer.Serialize(result);
			//Assert

			Assert.AreEqual(resultJson, expectedJson);
		}

		[Test]
		public async Task Filter_FilterPrice_ReturnsInPriceRange()
		{
			//Arrange
			var filter = new FilterViewModel
			{

				PriceLowerBound = 10,
				PriceUpperBound = 12,
			};
			var products = new List<Product>()
			{
				new Product
				{
					Price = 10.01,
				},
				new Product
				{
					Price = 10.5,
				},
				new Product
				{
					Price = 9,
				},
				new Product
				{
					Price = 12.01,
				},
				new Product
				{
					Name = "Test",
				},
			};
			var expected = new FilteredPageViewModel
			{
				ProductViewModels = new List<ProductViewModel>
				{
					new ProductViewModel
					{
						Price = 10.01,
					},
					new ProductViewModel
					{
						Price = 10.5,
					},
				},
				Filter = filter,
			};
			_productRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(products);

			//Act
			var result = await _productService.Filter(filter);
			var resultJson = JsonSerializer.Serialize(result);
			var expectedJson = JsonSerializer.Serialize(expected);

			//Assert
			Assert.That(resultJson, Is.EqualTo(expectedJson));
		}

		[Test]
		public async Task Filter_FilterNothing_ReturnsUnfiltered()
		{
			//Arrange
			var filter = new FilterViewModel();
			var products = new List<Product>()
			{
				new Product
				{
					Price = 10.01,
				},
				new Product
				{
					Price = 10.5,
				},
				new Product
				{
					Price = 9,
				},
				new Product
				{
					Price = 12.01,
				},
				new Product
				{
					Name = "Test",
				},
			};
			var expected = new FilteredPageViewModel
			{
				ProductViewModels = new List<ProductViewModel>
				{
					new ProductViewModel
					{
						Price = 10.01,
					},
					new ProductViewModel
					{
						Price = 10.5,
					},
					new ProductViewModel
					{
						Price = 9,
					},
					new ProductViewModel
					{
						Price = 12.01,
					},
					new ProductViewModel
					{
						Name = "Test",
					},
				},
				Filter = filter,
			};
			_productRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(products);

			//Act
			var result = await _productService.Filter(filter);
			var resultJson = JsonSerializer.Serialize(result);
			var expectedJson = JsonSerializer.Serialize(expected);

			//Assert
			Assert.That(resultJson, Is.EqualTo(expectedJson));
		}

		[Test]
		public async Task Filter_FilterAll_ReturnsFiltered()
		{
			//Arrange
			var filter = new FilterViewModel
			{
				SearchString = "Test",
				Category = Category.Mouses,
				PriceLowerBound = 10,
				PriceUpperBound = 15,
			};
			var products = new List<Product>()
			{
				new Product
				{
					Price = 10.01,
					Category = Category.Keyboards,
					Name = "Estt",
					Description = "StEss",
				},
				new Product
				{
					Price = 13,
					Category = Category.Mouses,
					Name = "Test3",
				},
				new Product
				{
					Price = 12.01,
					Category = Category.Mouses,
					Name = "est",
					Description = "StEs3s",
				},
				new Product
				{
					Name = "est1",
					Price = 13.2,
					Category = Category.Mouses,
					Description = "TestDescr",
				},
			};
			var expected = new FilteredPageViewModel
			{
				ProductViewModels = new List<ProductViewModel>
				{
					new ProductViewModel
					{
						Price = 13,
						Category = Category.Mouses,
						Name = "Test3",
					},
					new ProductViewModel
					{
						Name = "est1",
						Price = 13.2,
						Category = Category.Mouses,
						Description = "TestDescr",
					},
				},
				Filter = filter,
			};
			_productRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(products);

			//Act
			var result = await _productService.Filter(filter);
			var resultJson = JsonSerializer.Serialize(result);
			var expectedJson = JsonSerializer.Serialize(expected);

			//Assert
			Assert.That(resultJson, Is.EqualTo(expectedJson));
		}

		[Test]
		public async Task GetEditProductViewModel_FilterAll_ReturnsFiltered()
		{
			//Arrange
			var product = new Product
			{
				Id = "1",
				Name = "Test",
				Description = "TestDes",
				QuantityInStock = 1,
				Category = Category.Mouses,
				Price = 0.01,
			};
			var expected = new EditProductViewModel
			{
				Id = product.Id,
				Name = product.Name,
				Description = product.Description,
				QuantityInStock = product.QuantityInStock,
				Category = product.Category,
				Price = product.Price,
			};
			_productRepositoryMock.Setup(x => x.GetByIdAsync(product.Id)).ReturnsAsync(product);

			//act
			var result = await _productService.GetEditProductViewModel(product.Id);
			var resultJson = JsonSerializer.Serialize(result);
			var expectedJson = JsonSerializer.Serialize(expected);

			//Assert
			Assert.That(resultJson, Is.EqualTo(expectedJson));
		}

		[Test]
		public async Task UpdateProduct_PassedDummy_CallsUpdate()
		{
			//Arrange
			var dummy = new EditProductViewModel { };
			_productRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<string>())).ReturnsAsync(new Product());

			//Act
			_productService.UpdateProduct(dummy);

			//Assert
			_productRepositoryMock.Verify(x => x.Update(It.IsAny<Product>()), Times.Once);
		}

		[Test]
		public async Task GetProductViewModel_Test_Works()
		{
			//Arrange
			var product = new Product
			{
				Id = "2",
				QuantityInStock = 1,
				Category = Category.Other,
				Description = "Description",
				Name = "Test",
				Price = 12.0,
			};
			_productRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<string>())).ReturnsAsync(product);
			var expected = new ProductViewModel
			{
				Id = "2",
				QuantityInStock = 1,
				Category = Category.Other,
				Description = "Description",
				Name = "Test",
				Price = 12.0,
			};

			//Act
			var result = await _productService.GetProductViewModel("");
			var resultJson = JsonSerializer.Serialize(result);
			var expectedJson = JsonSerializer.Serialize(expected);

			//Assert
			_productRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<string>()), Times.AtLeastOnce);
			Assert.That(resultJson, Is.EqualTo(expectedJson));
		}

		[Test]
		public async Task DeleteProduct_PassedDummy_CallsDelete()
		{
			//Arrange
			_productRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<string>())).ReturnsAsync(new Product());

			//Act
			_productService.DeleteProduct(new string(""));

			//Assert
			_productRepositoryMock.Verify(x => x.Delete(It.IsAny<Product>()), Times.Once);
		}

		public ShoppingAppDbContext GetMemoryContext()
		{
			var options = new DbContextOptionsBuilder<ShoppingAppDbContext>()
										.UseInMemoryDatabase(databaseName: "InMemoryDatabase")
										.UseLazyLoadingProxies()
										.Options;
			return new ShoppingAppDbContext(options);
		}
	}
}