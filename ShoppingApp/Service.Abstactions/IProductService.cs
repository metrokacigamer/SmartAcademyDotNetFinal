﻿using Domain.Entities;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Abstactions
{
	public interface IProductService
	{
		Task<Product> AddProduct(AddProductViewModel model);
		void AdjustQuantity(IEnumerable<Item> items);
		Task<FilteredPageViewModel> Filter(FilterViewModel model, int currentPage, int pageSize);
		Task<IEnumerable<ProductViewModel>> GetProductsViewModels(string searchString, int currentPage, int pageSize);
		Task<EditProductViewModel> GetEditProductViewModel(string productId);
		Task UpdateProduct(EditProductViewModel model);
		Task<ProductViewModel> GetProductViewModel(string productId);
		Task DeleteProduct(string productId);
		Task<int> GetTotalPages(FilterViewModel filter, int pageSize);
		Task<IEnumerable<ProductViewModel>> SortBy(IEnumerable<ProductViewModel> productVMs, string sortBy, bool ascending);
	}
}
