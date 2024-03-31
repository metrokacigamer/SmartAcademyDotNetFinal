using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Service.Abstactions;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
	public class CartController: Controller
	{
		private readonly IServiceManager _serviceManager;

		public CartController(IServiceManager serviceManager)
		{
			_serviceManager = serviceManager;
		}

		[HttpPost]
		public async Task<IActionResult> AddToCart(string productId, int quantity)
		{
			if (User.Identity.IsAuthenticated)
			{
				var userId = _serviceManager.AccountService.GetCurrentUserId(User);
				var user = await _serviceManager.AccountService.GetById(userId);

				_serviceManager.CartService.AddToUserCart(user, productId, quantity);
			}
			else
			{
				_serviceManager.CartService.AddToGuestCart(productId, quantity);
			}
			return RedirectToAction("Index", "Home");
		}

		[HttpGet]
		public async Task<IActionResult> BuyItems()
		{
			var itemVMs = new List<ItemViewModel>();
			if (User.Identity.IsAuthenticated)
			{
				var userId = _serviceManager.AccountService.GetCurrentUserId(User);
				var user = await _serviceManager.AccountService.GetById(userId);

				itemVMs = _serviceManager.ItemService.GetItemViewModels(user.Cart.Items).ToList();

				_serviceManager.ProductService.AdjustQuantity(user.Cart.Items);
				_serviceManager.ItemService.BuyUserCartItems(user.Cart.Items);
			}
			else
			{
				var cartItems = await _serviceManager.CartService.BuyGuestCart();
				itemVMs = _serviceManager.ItemService.GetItemViewModels(cartItems).ToList();

				_serviceManager.ProductService.AdjustQuantity(cartItems);
			}
			return View(itemVMs);
		}

		[HttpPost]
		public IActionResult RemoveCartItem(string itemId)
		{
			if (!User.Identity.IsAuthenticated)
			{
				_serviceManager.CartService.RemoveItemFromGuestCart(itemId);
			}
			else
			{
				_serviceManager.ItemService.RemoveItem(itemId);
			}

			return RedirectToAction("CartItems", "Cart");
		}

		[HttpPost]
		public IActionResult ChangeQuantity(string itemId, int newQuantity)
		{
			if (!User.Identity.IsAuthenticated)
			{
				_serviceManager.CartService.ChangeGuestCartItemQuantity(itemId, newQuantity);
			}
			else
			{
				_serviceManager.CartService.ChangeItemQuantity(itemId, newQuantity);
			}

			return RedirectToAction("GetCartItems", "Cart");
		}

		[HttpGet]
		public async Task<IActionResult> GetCartItems()
		{
			if (User.Identity.IsAuthenticated)
			{
				var userId = _serviceManager.AccountService.GetCurrentUserId(User);
				var cart = await _serviceManager.CartService.GetUserCart(userId);
				var itemVMs = _serviceManager.ItemService.GetItemViewModels(cart.Items);

				return View(itemVMs);
			}
			else
			{
				var cart = await _serviceManager.CartService.GetCart();
				var itemVMs = _serviceManager.ItemService.GetItemViewModels(cart.Items);

				return View(itemVMs);
			}
		}
	}
}
