using Domain.Entities;
using Domain.Repositories;
using Microsoft.AspNetCore.Http;
using Service.Abstactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
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

					_repositoryManager.ImageRepository().Create(image);
				}
			}
		}
	}
}
