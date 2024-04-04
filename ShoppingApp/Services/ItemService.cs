using Domain.Entities;
using Domain.Repositories;
using Service.Abstactions;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
	internal class ItemService : IItemService
	{
		private readonly IRepositoryManager _repositoryManager;

		public ItemService(IRepositoryManager repositoryManager)
		{
			_repositoryManager = repositoryManager;
		}

		public async Task<IEnumerable<Item>> GetItems(string cartId)
		{
			return (await _repositoryManager.CartRepository.GetByIdAsync(cartId)).Items;
		}

		public void BuyUserCartItems(IEnumerable<Item>? items)
		{
			foreach(var item in items)
			{
				_repositoryManager.ItemRepository.Delete(item);
			}
		}

		public IEnumerable<ItemViewModel> GetItemViewModels(IEnumerable<Item> items)
		{
			var itemVMs = new List<ItemViewModel>();
			foreach(var item in items)
			{
				itemVMs.Add(new ItemViewModel
				{
					Id = item.Id,
					Description = item.Product.Description,
					Name = item.Product.Name,
					Price = item.Product.Price,
					Quantity = (item.Product.QuantityInStock > item.Quantity) ? item.Quantity : item.Product.QuantityInStock,
					ImagePaths = item.Product.Images.Select(x => x.ImagePath),
				});
			}

			return itemVMs;
		}

		public async Task RemoveItem(string itemId)
		{
			var item = await _repositoryManager.ItemRepository.GetByIdAsync(itemId);
			_repositoryManager.ItemRepository.Delete(item);
		}
	}
}
