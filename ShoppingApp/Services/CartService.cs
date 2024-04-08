using Domain.Entities;
using Domain.Repositories;
using Domain.Wrappers;
using Microsoft.AspNetCore.Http;
using Service.Abstactions;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;


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

		public async Task<Cart> GetCart()//tested
		{
			var cartJson = _session.GetString(_key);
			if (cartJson == null)
			{
				return new Cart
				{
					Id = Guid.NewGuid().ToString(),
					Items = new List<Item>(),
				};
			}
			var opt = new JsonSerializerOptions()
			{
				ReferenceHandler = ReferenceHandler.Preserve,
				WriteIndented = true,
			};
			var cart = JsonSerializer.Deserialize<Cart>(cartJson, opt);
			foreach (var item in cart.Items)
			{
				item.Product = await _repositoryManager.ProductRepository.GetByIdAsync(item.ProductId);
			}
			SaveCart(cart);

			return cart;
		}

		public async Task<Cart> GetUserCart(string userId)//tested
		{
			return (await _repositoryManager.CartRepository.GetAllAsync()).First(x => x.UserId == userId);
		}


		public async Task AddToUserCart(AppUser user, string productId, int quantity)//tested
		{
			var product = await _repositoryManager.ProductRepository.GetByIdAsync(productId);
			CheckQuantity(product, quantity);

			if(user.Cart == null)
			{
				_repositoryManager.CartRepository.Create(new Cart
				{
					User = user
				});
			}

			var cartId = user.Cart.Id;
			var cart = await _repositoryManager.CartRepository.GetByIdAsync(cartId);

			if (cart.Items != null && cart.Items.Any(x => x.ProductId == productId))
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

		public Item UpdateQuantity(Cart cart, Product product, int quantity)//tested
		{
			CheckQuantity(product, quantity);

			var item = cart.Items.First(x => x.ProductId == product.Id);
			item.Product = product;
			item.Quantity = (product.QuantityInStock > quantity) ? quantity : product.QuantityInStock;

			return item;
		}

		public void CheckQuantity(Product product, int quantity)//tested
		{
			if (product.QuantityInStock < quantity)
			{
				throw new ArgumentException("Not Enough items in stock");
			}
		}

		public async Task AddToGuestCart(string productId, int quantity)//tested
		{
			var product = await _repositoryManager.ProductRepository.GetByIdAsync(productId);
			CheckQuantity(product, quantity);

			var cart = await GetCart();

			if (cart.Items!.Any(x => x.ProductId == productId))
			{
				var item = cart.Items.First(x => x.ProductId == productId);
				UpdateQuantity(cart, product, quantity + item.Quantity);//tested
			}
			else
			{
				cart.Items = cart.Items!.Append(new Item
				{
					Id = Guid.NewGuid().ToString(),
					Product = await _repositoryManager.ProductRepository.GetByIdAsync(productId),
					ProductId = product.Id,
					Cart = cart,
					CartId = cart.Id,
					Quantity = quantity,
				});
			}

			SaveCart(cart);
		}

		public void SaveCart(Cart cart)//tested
		{
			var opt = new JsonSerializerOptions()
			{
				ReferenceHandler = ReferenceHandler.Preserve,
				WriteIndented = true,
			};
			var cartString = JsonSerializer.Serialize(cart, opt);
			_session.SetString(_key, cartString);
		}

		public async Task<IEnumerable<Item>> BuyGuestCart()//tested
		{
			var cart = await GetCart();
			var items = new List<Item>(cart.Items);

			cart.Items = new List<Item>();

			SaveCart(cart);

			return items;
		}

		public async Task RemoveItemFromGuestCart(string itemId)//testesd
		{
			var cart = await GetCart();
			var tempList = cart.Items.ToList();
			tempList.RemoveAll(x => x.Id == itemId);
			cart.Items = tempList;

			SaveCart(cart);
		}

		public async Task ChangeGuestCartItemQuantity(string itemId, int newQuantity)//tested
		{
			var cart = await GetCart();
			var productId = cart.Items.FirstOrDefault(x => x.Id == itemId).ProductId;
			var product = await _repositoryManager.ProductRepository.GetByIdAsync(productId);

			UpdateQuantity(cart, product, newQuantity);
			
			SaveCart(cart);
		}

		public async Task ChangeItemQuantity(string itemId, int newQuantity)//tested
		{
			var item = await _repositoryManager.ItemRepository.GetByIdAsync(itemId);

			UpdateQuantity(item.Cart, item.Product, newQuantity);

			_repositoryManager.ItemRepository.Update(item); 
		}
	}
}
