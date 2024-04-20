
using Domain.Entities;
using Domain.Repositories;
using Domain.Wrappers;
using Microsoft.AspNetCore.Http;
using Moq;
using Persistence.Repositories;
using System.Text;
using System.Text.Json;

namespace Services.Tests
{
	internal class CartServiceTests
	{
		private Mock<IRepositoryManager> _repositoryManagerMock;
		private Mock<ISessionWrapper> _sessionMock;
		private Mock<IRepository<Cart>> _cartRepositoryMock;
		private Mock<IRepository<Product>> _productRepositoryMock;
		private Mock<IRepository<Item>> _itemRepositoryMock;

		private CartService _cartService;

		[SetUp]
		public void Setup()
		{
			_repositoryManagerMock = new Mock<IRepositoryManager>();
			_cartRepositoryMock = new Mock<IRepository<Cart>>();
			_productRepositoryMock = new Mock<IRepository<Product>>();
			_itemRepositoryMock = new Mock<IRepository<Item>>();

			_repositoryManagerMock.Setup(x => x.CartRepository).Returns(_cartRepositoryMock.Object);
			_repositoryManagerMock.Setup(x => x.ProductRepository).Returns(_productRepositoryMock.Object);
			_repositoryManagerMock.Setup(x => x.ItemRepository).Returns(_itemRepositoryMock.Object);

			_sessionMock = new Mock<ISessionWrapper>();

			_cartService = new CartService(_repositoryManagerMock.Object, _sessionMock.Object);
		}

		[Test]
		public async Task GetCart_CartExists_ReturnsCart()
		{
			//Arrange
			var cart = new Cart
			{
				Items = new List<Item>(),
			};
			_sessionMock.Setup(x => x.GetString(It.IsAny<string>())).Returns(JsonSerializer.Serialize(cart));

			//Act
			var result = await _cartService.GetCart();
			var resultJson = JsonSerializer.Serialize(result);
			var expectedJson = JsonSerializer.Serialize(cart);

			//Assert
			Assert.That(resultJson, Is.EqualTo(expectedJson));
		}

		[Test]
		public async Task GetCart_CartDoesntExists_CreatesNewCart()
		{
			//Arrange

			_sessionMock.Setup(x => x.GetString(It.IsAny<string>())).Returns((string)null);

			//Act
			var result = await _cartService.GetCart();
			var resultJson = JsonSerializer.Serialize(result);

			//Assert
			Assert.IsNotNull(result);
			Assert.IsNotNull(result.Id);
			Assert.IsNotEmpty(result.Id);
		}

		[Test]
		public async Task GetUserCart_PassedUserId_ReturnsUserCart()
		{
			//Arrange
			var id = "1";
			var cart = new Cart
			{
				UserId = id,
			};
			_cartRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<Cart>() { cart });

			//Act
			var result = await _cartService.GetUserCart(id);

			//Assert
			Assert.That(result, Is.EqualTo(cart));
		}

		[Test]
		public async Task AddToUserCart_NoSuchItemInCart_ItemAddedToCart()
		{
			//Arrange
			var user = new AppUser
			{
				Id = "0",
			};
			var product = new Product
			{
				Id = "1",
			};
			var cart = new Cart
			{
				Id = "2",
				User = user,
				UserId = user.Id,
				Items = new List<Item>(),
			};
			user.Cart = cart;
			_productRepositoryMock.Setup(x => x.GetByIdAsync(product.Id)).ReturnsAsync(product);
			_cartRepositoryMock.Setup(x => x.GetByIdAsync(cart.Id)).ReturnsAsync(cart);

			//Act
			await _cartService.AddToUserCart(user, product.Id, 0);

			//Assert
			_repositoryManagerMock.Verify(x => x.ItemRepository, Times.Once);
		}

		[Test]
		public async Task AddToUserCart_SuchItemIsInCart_ItemQuantityUpdated()
		{
			//Arrange
			var user = new AppUser
			{
				Id = "0",
			};
			var product = new Product
			{
				Id = "1",
				QuantityInStock = 3,
			};
			var cart = new Cart
			{
				Id = "2",
				User = user,
				UserId = user.Id,
				Items = new List<Item>()
				{
					new Item
					{
						ProductId = product.Id,
					}
				},
			};
			user.Cart = cart;
			_productRepositoryMock.Setup(x => x.GetByIdAsync(product.Id)).ReturnsAsync(product);
			_cartRepositoryMock.Setup(x => x.GetByIdAsync(cart.Id)).ReturnsAsync(cart);

			//Act
			await _cartService.AddToUserCart(user, product.Id, 1);
			await _cartService.AddToUserCart(user, product.Id, 2);

			//Assert
			_repositoryManagerMock.Verify(x => x.ItemRepository, Times.Exactly(2));
			Assert.That(cart.Items.First().Quantity, Is.EqualTo(product.QuantityInStock));
		}

		[Test]
		public async Task AddToGuestCart_NoSuchItemInCart_ItemAddedToCart()
		{
			//Arrange
			var product = new Product
			{
				Id = "1",
				QuantityInStock = 2,
			};
			var cart = new Cart
			{
				Id = "2",
				Items = new List<Item>(),
			};

			_productRepositoryMock.Setup(x => x.GetByIdAsync(product.Id)).ReturnsAsync(product);
			_cartRepositoryMock.Setup(x => x.GetByIdAsync(cart.Id)).ReturnsAsync(cart);

			_sessionMock.Setup(x => x.GetString(It.IsAny<string>())).Returns(JsonSerializer.Serialize(cart));

			//Act
			await _cartService.AddToGuestCart(product.Id, 1);

			//Assert
			_sessionMock.Verify(x => x.SetString(It.IsAny<string>(), It.IsAny<string>()), Times.AtLeast(1));
		}

		[Test]
		public async Task AddToGuestCart_SuchItemIsInCart_ItemQuantityUpdated()
		{
			//Arrange
			var product = new Product
			{
				Id = "1",
				QuantityInStock = 2,
			};
			var cart = new Cart
			{
				Id = "2",
				Items = new List<Item>()
				{
					new Item
					{
						ProductId = product.Id,
					},
				},
			};

			_productRepositoryMock.Setup(x => x.GetByIdAsync(product.Id)).ReturnsAsync(product);
			_cartRepositoryMock.Setup(x => x.GetByIdAsync(cart.Id)).ReturnsAsync(cart);

			_sessionMock.Setup(x => x.GetString(It.IsAny<string>())).Returns(JsonSerializer.Serialize(cart));

			//Act
			await _cartService.AddToGuestCart(product.Id, 1);
			await _cartService.AddToGuestCart(product.Id, 2);

			//Assert
			_sessionMock.Verify(x => x.SetString(It.IsAny<string>(), It.IsAny<string>()), Times.AtLeast(2));
		}

		[Test]
		public async Task BuyGuestCart_GetsCart_SavesCartAndReturnsItems()
		{
			//Arrange
			var cart = new Cart
			{
				Items = new List<Item>(),
			};

			_sessionMock.Setup(x => x.GetString(It.IsAny<string>())).Returns(JsonSerializer.Serialize(cart));

			//Act
			var result = await _cartService.BuyGuestCart();
			var resultJson = JsonSerializer.Serialize(result);
			var expectedJson = JsonSerializer.Serialize(cart.Items);

			//Assert
			_sessionMock.Verify(x => x.SetString(It.IsAny<string>(), It.IsAny<string>()), Times.AtLeast(1));
			Assert.That(resultJson, Is.EqualTo(expectedJson));
		}

		[Test]
		public async Task RemoveItemFromGuestCart_Test_Works()
		{
			//Arrange
			var cart = new Cart
			{
				Items = new List<Item>()
				{
					new Item
					{
						Id = "3",
					}
				},
			};
			_sessionMock.Setup(x => x.GetString(It.IsAny<string>())).Returns(JsonSerializer.Serialize(cart));

			//Act
			await _cartService.RemoveItemFromGuestCart(cart.Items.First().Id);

			//Assert
			_sessionMock.Verify(x => x.SetString(It.IsAny<string>(), It.IsAny<string>()), Times.AtLeastOnce);
		}

		[Test]
		public async Task ChangeGuestCartItemQuantity_PassedValidQuantity_CallsSetString()
		{
			//Arrange
			var product = new Product
			{
				Id = "1",
				QuantityInStock = 2,
				CartItems = new List<Item>(),
			};
			var cart = new Cart
			{
				Items = new List<Item>()
				{
					new Item
					{
						Id = "3",
						Product = product,
						ProductId = product.Id,
						Quantity = 1,
					}
				},
			};
			product.CartItems.Append(cart.Items.First());

			_sessionMock.Setup(x => x.GetString(It.IsAny<string>())).Returns(JsonSerializer.Serialize(cart));
			_productRepositoryMock.Setup(x => x.GetByIdAsync(product.Id)).ReturnsAsync(product);

			//Act
			await _cartService.ChangeGuestCartItemQuantity(cart.Items.First().Id, 2);

			//Assert
			_sessionMock.Verify(x => x.SetString(It.IsAny<string>(), It.IsAny<string>()), Times.AtLeastOnce);
			_productRepositoryMock.Verify(x => x.GetByIdAsync(product.Id), Times.AtLeastOnce);
		}

		[Test]
		public async Task ChangeItemQuantity_PassedValidQuantity_UpdatesItem()
		{
			//Arrange
			var product = new Product
			{
				Id = "1",
				QuantityInStock = 2,
			};
			var cart = new Cart
			{
				Id = "0",
				Items = new List<Item>()
				{
					new Item
					{
						Product = product,
						ProductId = product.Id,
						Id = "2",
						Quantity = 1,
					}
				},
			};
			cart.Items.First().Cart = cart;
			cart.Items.First().CartId = cart.Id;

			_cartRepositoryMock.Setup(x => x.GetByIdAsync(cart.Id)).ReturnsAsync(cart);
			_itemRepositoryMock.Setup(x => x.GetByIdAsync(cart.Items.First().Id)).ReturnsAsync(cart.Items.First());

			//Act
			await _cartService.ChangeItemQuantity("2", 2);

			//Assert
			_itemRepositoryMock.Verify(x => x.Update(cart.Items.First()), Times.Once);
			Assert.That(cart.Items.First().Quantity , Is.EqualTo(2));
		}
	}
}
