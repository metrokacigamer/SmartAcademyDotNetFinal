using Domain.Entities;
using Domain.Repositories;
using Domain.Wrappers;
using Microsoft.AspNetCore.Http;
using Service.Abstactions;
using System.Text;
using System.Text.Json;


namespace Services
{
	internal class CartService : ICartService
	{
		private readonly IRepositoryManager _repositoryManager;
		private readonly ISessionWrapper _session;
		private readonly string _key = "cartKey";

		public CartService(IRepositoryManager repositoryManager, ISessionWrapper session)
		{
			_repositoryManager = repositoryManager;
			_session = session;
		}

		public async Task<Cart> GetCart()
		{
			var cartJson = _session.GetString(_key);
			if (cartJson == null)
			{
				return new Cart
				{
					Id = Guid.NewGuid().ToString(),
				};
			}

			var cart = JsonSerializer.Deserialize<Cart>(cartJson);
			foreach (var item in cart.Items)
			{
				item.Product = await _repositoryManager.ProductRepository.GetByIdAsync(item.ProductId);
			}
			SaveCart(cart);

			return cart;
		}

		public async Task<Cart> GetUserCart(string userId)
		{
			return (await _repositoryManager.CartRepository.GetAllAsync()).First(x => x.UserId == userId);
		}


		public async Task AddToUserCart(AppUser user, string productId, int quantity)
		{
			var product = await _repositoryManager.ProductRepository.GetByIdAsync(productId);
			CheckQuantity(product, quantity);

			var cartId = user.Cart.Id;
			var cart = await _repositoryManager.CartRepository.GetByIdAsync(cartId);

			if (cart.Items.Any(x => x.ProductId == productId))
			{
				var item = cart.Items.First(x => x.ProductId == productId);
				item = UpdateQuantity(cart, product, quantity + item.Quantity);
				_repositoryManager.ItemRepository.Update(item);
			}
			else
			{
				var item = new Item
				{
					Cart = cart,
					Product = product,
					Quantity = quantity
				};
				_repositoryManager.ItemRepository.Create(item);
			}
		}

		public Item UpdateQuantity(Cart cart, Product product, int quantity)
		{
			CheckQuantity(product, quantity);

			var item = cart.Items.First(x => x.ProductId == product.Id);
			item.Product = product;
			item.Quantity = (product.QuantityInStock > quantity) ? quantity : product.QuantityInStock;

			return item;
		}

		public async Task CheckQuantity(Product product, int quantity)
		{
			if (product.QuantityInStock < quantity)
			{
				throw new ArgumentException("Not Enough items in stock");
			}
		}

		public async Task AddToGuestCart(string productId, int quantity)
		{
			var product = await _repositoryManager.ProductRepository.GetByIdAsync(productId);
			CheckQuantity(product, quantity);

			var cart = await GetCart();

			if (cart.Items!.Any(x => x.ProductId == productId))
			{
				UpdateQuantity(cart, product, quantity);
			}
			else
			{
				cart.Items!.Append(new Item
				{
					Product = await _repositoryManager.ProductRepository.GetByIdAsync(productId),
					ProductId = product.Id,
					Cart = cart,
					CartId = cart.Id,
					Quantity = quantity,
				});
			}

			SaveCart(cart);
		}

		public void SaveCart(Cart cart)
		{
			var cartString = JsonSerializer.Serialize(cart);
			_session.SetString(_key, cartString);
		}

		public async Task<IEnumerable<Item>> BuyGuestCart()
		{
			var cart = await GetCart();
			var items = new List<Item>(cart.Items);

			cart.Items = new List<Item>();

			SaveCart(cart);

			return items;
		}

		public async Task RemoveItemFromGuestCart(string itemId)
		{
			var cart = await GetCart();
			var tempList = cart.Items.ToList();
			tempList.RemoveAll(x => x.Id == itemId);
			cart.Items = tempList;

			SaveCart(cart);
		}

		public async Task ChangeGuestCartItemQuantity(string itemId, int newQuantity)
		{
			var cart = await GetCart();
			var productId = cart.Items.FirstOrDefault(x => x.Id == itemId).ProductId;
			var product = await _repositoryManager.ProductRepository.GetByIdAsync(productId);

			UpdateQuantity(cart, product, newQuantity);
			
			SaveCart(cart);
		}

		public async Task ChangeItemQuantity(string itemId, int newQuantity)
		{
			var item = await _repositoryManager.ItemRepository.GetByIdAsync(itemId);

			UpdateQuantity(item.Cart, item.Product, newQuantity);

			_repositoryManager.ItemRepository.Update(item); 
		}
	}
}
