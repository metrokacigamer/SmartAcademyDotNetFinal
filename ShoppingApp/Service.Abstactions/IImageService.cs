using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Abstactions
{
	public interface IImageService
	{
		Task AddImages(IEnumerable<IFormFile> productImages, Product product);
		Task<IEnumerable<ImageViewModel>> GetImageViewModels(string productId);
		Task UpdateProductImages(EditProductViewModel model);
	}
}
