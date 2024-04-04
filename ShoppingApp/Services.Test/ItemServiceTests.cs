using Domain.Entities;
using Domain.Repositories;
using Moq;
using Shared.Models;
using System.Text.Json;


namespace Services.Tests
{
	internal class ItemServiceTests
	{
		private Mock<IRepositoryManager> _repositoryManagerMock;
		private Mock<IRepository<Item>> _itemRepositoryMock;
		private Mock<IRepository<Cart>> _cartRepositoryMock;

		private ItemService _itemService;

		[SetUp]
		public void Setup()
		{
			_repositoryManagerMock = new Mock<IRepositoryManager>();
			_itemRepositoryMock = new Mock<IRepository<Item>>();
			_cartRepositoryMock = new Mock<IRepository<Cart>>();

			_repositoryManagerMock.Setup(x => x.CartRepository).Returns(_cartRepositoryMock.Object);
			_repositoryManagerMock.Setup(x => x.ItemRepository).Returns(_itemRepositoryMock.Object);

			_itemService = new ItemService(_repositoryManagerMock.Object);
		}

		[Test]
		public async Task BuyUserCartItems_PassedItems_CallsDelete()
		{
			//Arrange
			var cart = new Cart();
			var items = new List<Item>()
			{
				new Item(),
				new Item(),
				new Item(),
				new Item(),
			};
			cart.Items = items;
			_cartRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<string>())).ReturnsAsync(cart);

			//Act
			_itemService.BuyUserCartItems(items);

			//Assert
			_itemRepositoryMock.Verify(x => x.Delete(It.IsAny<Item>()), Times.Exactly(4));
		}

		[Test]
		public async Task GetItemViewModels_PassedItems_ReturnsItemModels()
		{
			//Arrange
			var product = new Product()
			{
				Name = "test",
				Description = "testdescr",
				Price = 3.2,
				Category = Domain.Enums.Category.Mouses,
				Id = "0",
				Images = new List<Image>()
				{
					new Image()
					{
						ImagePath = "",
					}
				},
			};
			var items = new List<Item>()
			{
				new Item
				{
					Product = product,
					ProductId = product.Id,
					Id = "1"
				},
				new Item
				{
					Product = product,
					ProductId = product.Id,
					Id = "2"
				},
				new Item
				{
					Product = product,
					ProductId = product.Id,
					Id = "3"
				},
				new Item
				{
					Product = product,
					ProductId = product.Id,
					Id = "4"
				},
			};
			product.CartItems = items;
			var expected = new List<ItemViewModel>()
			{
				new ItemViewModel
				{
					ProductId = product.Id,
					Id = "1",
					Name = "test",
					Description = "testdescr",
					Price = 3.2,
					Category = Domain.Enums.Category.Mouses,
					ImagePaths = new List<string>(){""},
				},
				new ItemViewModel
				{
					ProductId = product.Id,
					Id = "2",
					Name = "test",
					Description = "testdescr",
					Price = 3.2,
					Category = Domain.Enums.Category.Mouses,
					ImagePaths = new List<string>(){""},
				},
				new ItemViewModel
				{
					ProductId = product.Id,
					Id = "3",
					Name = "test",
					Description = "testdescr",
					Price = 3.2,
					Category = Domain.Enums.Category.Mouses,
					ImagePaths = new List<string>(){""},
				},
				new ItemViewModel
				{
					ProductId = product.Id,
					Id = "4",
					Name = "test",
					Description = "testdescr",
					Price = 3.2,
					Category = Domain.Enums.Category.Mouses,
					ImagePaths = new List<string>(){""},
				},
			};

			//Act
			var result = _itemService.GetItemViewModels(items);
			var resultJson = JsonSerializer.Serialize(result);
			var expectedJson = JsonSerializer.Serialize(expected);

			//Assert
			Assert.That(resultJson, Is.EqualTo(expectedJson));
		}
	}
}
