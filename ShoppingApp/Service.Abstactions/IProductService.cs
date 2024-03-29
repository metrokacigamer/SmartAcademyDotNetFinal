using Domain.Entities;
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
	}
}
