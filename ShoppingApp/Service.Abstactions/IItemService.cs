using Domain.Entities;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Abstactions
{
	public interface IItemService
	{
		void BuyUserCartItems(IEnumerable<Item>? items);
		Task<IEnumerable<Item>> GetItems(string cartId);
		IEnumerable<ItemViewModel> GetItemViewModels(IEnumerable<Item> items);
		Task RemoveItem(string itemId);
	}
}
