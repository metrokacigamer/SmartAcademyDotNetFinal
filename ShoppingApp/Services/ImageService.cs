using Domain.Entities;
using Domain.Exceptions;
using Domain.Repositories;
using Domain.Wrappers;
using Microsoft.AspNetCore.Http;
using Service.Abstactions;
using Shared.Models;


namespace Services
{
	internal class ImageService : IImageService
	{
		private readonly IRepositoryManager _repositoryManager;
		private readonly IFileStreamWrapper _fileStream;

		public ImageService(IRepositoryManager repositoryManager, IFileStreamWrapper fileStream)
		{
			_repositoryManager = repositoryManager;
			_fileStream = fileStream;
		}

		public async Task AddImages(IEnumerable<IFormFile> productImages, Product product)//tested
		{
			foreach (var imageFile in productImages)
			{
				CheckFile(imageFile);

				var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
				var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", fileName);
				var relativePath = $@"~/images/{fileName}";

				await _fileStream.CopyToAsync(imageFile, imagePath);

				var image = new Image
				{
					Product = product,
					ImagePath = relativePath,
				};

				_repositoryManager.ImageRepository.Create(image);

			}
		}

		public void CheckFile(IFormFile imageFile)//tested
		{
			if (imageFile == null)
			{
				throw new InvalidFileTypeException(message: "No file attached");
			}
			var fileExtension = Path.GetExtension(imageFile.FileName).ToLower();

			if (fileExtension != ".jpg" && fileExtension != ".jpeg" && fileExtension != ".png")
			{
				throw new InvalidFileTypeException(extension: fileExtension);
			}
		}
		

		public async Task UpdateProductImages(EditProductViewModel model)//tested
		{
			if (model.NewImages != null && model.NewImages.Any())
			{
				var product = await _repositoryManager.ProductRepository.GetByIdAsync(model.Id);
				await AddImages(model.NewImages, product);
			}

			if (model.RemovedImageIds != null && model.RemovedImageIds.Any())
			{
				foreach (var imageId in model.RemovedImageIds)
				{
					var product = await _repositoryManager.ProductRepository.GetByIdAsync(model.Id);
					var image = product.Images.First(x => x.Id == imageId);
					_repositoryManager.ImageRepository.Delete(image);
				}
			}
		}

		public async Task<IEnumerable<ImageViewModel>> GetImageViewModels(string productId)//tested
		{
			var imageVMs = new List<ImageViewModel>();
			var product = await _repositoryManager.ProductRepository.GetByIdAsync(productId);

			foreach (var image in product.Images)
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
