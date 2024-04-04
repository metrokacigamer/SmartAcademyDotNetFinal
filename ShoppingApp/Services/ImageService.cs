using Domain.Entities;
using Domain.Repositories;
using Microsoft.AspNetCore.Http;
using Service.Abstactions;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace services
{
	internal class ImageService: IImageService
	{
		private readonly IRepositoryManager _repositoryManager;

		public ImageService(IRepositoryManager repositoryManager)
        {
			_repositoryManager = repositoryManager;
		}

		public async Task AddImages(IEnumerable<IFormFile> productImages, Product product)
		{
			foreach (var imageFile in productImages)
			{
				var fileExtension = Path.GetExtension(imageFile.FileName).ToLower();

				if (imageFile != null && imageFile.Length > 0 && !(fileExtension != ".jpg" && fileExtension != ".jpeg" && fileExtension != ".png"))
				{
					var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
					var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", fileName);
					var relativePath = $@"~/images/{fileName}";

					using (var stream = new FileStream(imagePath, FileMode.Create))
					{
						await imageFile.CopyToAsync(stream);
					}

					var image = new Image
					{
						Product = product,
						ImagePath = relativePath,
					};

					_repositoryManager.ImageRepository.Create(image);
				}
			}
		}

		public async Task UpdateProductImages(EditProductViewModel model)
		{
			if(model.NewImages.Any())
			{
				var product = await _repositoryManager.ProductRepository.GetByIdAsync(model.Id);
				await AddImages(model.NewImages, product);
			}

			if(model.RemovedImageIds.Any())
			{
				foreach(var imageId in model.RemovedImageIds)
				{
					var image = await _repositoryManager.ImageRepository.GetByIdAsync(imageId);
					_repositoryManager.ImageRepository.Delete(image);
				}
			}
		}

		public async Task<IEnumerable<ImageViewModel>> GetImageViewModels(string productId)
		{
			var imageVMs = new List<ImageViewModel>();
			var product = await _repositoryManager.ProductRepository.GetByIdAsync(productId);

			foreach(var image in product.Images)
			{
				imageVMs.Add(new ImageViewModel
				{
					Id = image.Id,
					ImagePath = image.ImagePath,
					ProductId = productId,
				});
			}

			return imageVMs;
		}
	}
}
