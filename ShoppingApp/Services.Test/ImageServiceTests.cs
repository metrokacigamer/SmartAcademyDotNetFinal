using Domain.Repositories;
using Moq;
using Domain.Entities;
using Domain.Wrappers;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Http;
using Domain.Exceptions;
using Shared.Models;
using System.Text.Json;


namespace Services.Tests
{
	internal class ImageServiceTests
	{
		private Mock<IRepositoryManager> _repositoryManagerMock;
		private Mock<IRepository<Image>> _imageRepositoryMock;
		private Mock<IRepository<Product>> _productRepositoryMock;
		private Mock<IFileStreamWrapper> _fileStreamMock;
		private ImageService _imageService;

		[SetUp]
		public void Setup()
		{
			_repositoryManagerMock = new Mock<IRepositoryManager>();
			_imageRepositoryMock = new Mock<IRepository<Image>>();
			_productRepositoryMock = new Mock<IRepository<Product>>();
			_fileStreamMock = new Mock<IFileStreamWrapper>();

			_repositoryManagerMock.Setup(x => x.ImageRepository).Returns(_imageRepositoryMock.Object);
			_repositoryManagerMock.Setup(x => x.ProductRepository).Returns(_productRepositoryMock.Object);

			_imageService = new ImageService(_repositoryManagerMock.Object, _fileStreamMock.Object);
		}

		[Test]
		public async Task AddImages_ImproperExtension_ThrowsInvalidFileTypeException()
		{
			//Arrange
			var files = new List<IFormFile>()
			{
				new FormFile(null, 0, 0, "test", ".sda")
			};
			var files2 = new List<IFormFile>()
			{
				(FormFile)null,
			};
			var product = new Product
			{
				Id = "1",
			};
			//Act && Assert
			Assert.ThrowsAsync<InvalidFileTypeException>(async () => await _imageService.AddImages(files,product));
			Assert.ThrowsAsync<InvalidFileTypeException>(async () => await _imageService.AddImages(files2, product));
		}

		[Test]
		public async Task AddImages_ProperExtension_CallsCopyToAsyncAndCreate()
		{
			//Arrange
			var files = new List<IFormFile>()
			{
				new FormFile(null, 0, 0, "test", ".jpeg")
			};
			var product = new Product
			{
				Id = "1",
			};

			//Act
			await _imageService.AddImages(files,product);

			//Assert
			_fileStreamMock.Verify(x => x.CopyToAsync(It.IsAny<IFormFile>(), It.IsAny<string>()), Times.Once);
			_imageRepositoryMock.Verify(x => x.Create(It.IsAny<Image>()), Times.Once);
		}

		[Test]
		public async Task UpdateProductImages_NoChanges_DoesnNothing()
		{
			//Arrange
			var model = new EditProductViewModel
			{
				NewImages = new List<IFormFile>(),
				RemovedImageIds = new List<string>(),
			};

			//Act
			await _imageService.UpdateProductImages(model);

			//Assert
			_fileStreamMock.Verify(x => x.CopyToAsync(It.IsAny<IFormFile>(), It.IsAny<string>()), Times.Never);
			_imageRepositoryMock.Verify(x => x.Delete(It.IsAny<Image>()), Times.Never);
			_imageRepositoryMock.Verify(x => x.Create(It.IsAny<Image>()), Times.Never);
		}

		[Test]
		public async Task UpdateProductImages_ProductDoesntContainImageToDelete_ThrowsInvalidOperationException()
		{
			//Arrange
			var model = new EditProductViewModel
			{
				NewImages = new List<IFormFile>(),
				RemovedImageIds = new List<string>()
				{
					"2122",
				},
			};
			var product = new Product
			{
				Images = new List<Image>()
				{
					new Image
					{
						Id = "2",
					}
				},
			};

			_productRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<string>())).ReturnsAsync(product);
			_imageRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<string>())).ReturnsAsync(new Image());

			//Act && Assert
			Assert.ThrowsAsync<InvalidOperationException>(async () => await _imageService.UpdateProductImages(model));
		}

		[Test]
		public async Task UpdateProductImages_ChangesPresent_AppliesChanges()
		{
			//Arrange
			var model = new EditProductViewModel
			{
				NewImages = new List<IFormFile>()                
				{
					new FormFile(null, 0, 0, "test", ".jpeg"),
				},
				RemovedImageIds = new List<string>()
				{
					"2",
				},
			};
			var product = new Product
			{
				Images = new List<Image>()
				{
					new Image
					{
						Id = "2",
					}
				},
			};

			_productRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<string>())).ReturnsAsync(product);
			_imageRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<string>())).ReturnsAsync(new Image());

			//Act
			await _imageService.UpdateProductImages(model);

			//Assert
			_fileStreamMock.Verify(x => x.CopyToAsync(It.IsAny<IFormFile>(), It.IsAny<string>()), Times.Once);
			_imageRepositoryMock.Verify(x => x.Delete(It.IsAny<Image>()), Times.Once);
			_imageRepositoryMock.Verify(x => x.Create(It.IsAny<Image>()), Times.Once);
		}

		[Test]
		public async Task GetImageViewModels_PassedProductId_ReturnsProductImageModels()
		{
			//Arrange
			var product = new Product
			{
				Id = "1",
			};

			var images = new List<Image>()
			{
				new Image
				{
					Id = "12",
					ImagePath = "testpath1",
					Product = product,
					ProductId = product.Id,
				},
				new Image
				{
					Id = "22",
					ImagePath = "testpath2",
					Product = product,
					ProductId = product.Id,
				},
				new Image
				{
					Id = "32",
					ImagePath = "testpath3",
					Product = product,
					ProductId = product.Id,
				},
			};
			product.Images = images;
			var expected = new List<ImageViewModel>()
			{
				new ImageViewModel
				{
					Id = "12",
					ImagePath = "testpath1",
					ProductId = product.Id,
				},
				new ImageViewModel
				{
					Id = "22",
					ImagePath = "testpath2",
					ProductId = product.Id,
				},
				new ImageViewModel
				{
					Id = "32",
					ImagePath = "testpath3",
					ProductId = product.Id,
				},
			};
			_productRepositoryMock.Setup(x => x.GetByIdAsync("1")).ReturnsAsync(product);

			//Act
			var result = await _imageService.GetImageViewModels("1");
			var resultJson = JsonSerializer.Serialize(result);
			var expectedJson = JsonSerializer.Serialize(expected);

			//Assert
			Assert.That(resultJson, Is.EqualTo(expectedJson));
		}
	}
}
