using Domain.Entities;
using System.Security.Claims;

namespace Service.Abstactions
{
	public interface ICartService
	{
		Task AddToGuestCart(string productId, int quantity);
		Task AddToUserCart(AppUser userId, string productId, int quantity);
		Task<IEnumerable<Item>> BuyGuestCart();
		Task ChangeGuestCartItemQuantity(string itemId, int newQuantity);
		Task ChangeItemQuantity(string itemId, int newQuantity);
		Task<Cart> GetCart();
		Task<Cart> GetUserCart(string userId);
		Task RemoveItemFromGuestCart(string itemId);
		void SaveCart(Cart cart);
	}
}
