using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Abstactions;
using Shared.Models;

namespace Presentation.Controllers
{
	public class ProductController : Controller
	{
		private readonly IServiceManager _serviceManager;

		public ProductController(IServiceManager serviceManager)
		{
			_serviceManager = serviceManager;
		}

		[HttpGet]
		public async Task<IActionResult> GetProduct(string productId)
		{
			var productVM = await _serviceManager.ProductService.GetProductViewModel(productId);
			productVM.Images = await _serviceManager.ImageService.GetImageViewModels(productId);
			return View(productVM);
		}

		[Authorize(Roles = "Admin")]
		[HttpGet]
		public IActionResult AddProduct()
		{
			return View();
		}

		[Authorize(Roles = "Admin")]
		[HttpPost]
		public async Task<IActionResult> AddProduct(AddProductViewModel model)
		{
			if(ModelState.IsValid)
			{
				var product = await _serviceManager.ProductService.AddProduct(model);
				await _serviceManager.ImageService.AddImages(model.ProductImages, product);
				return RedirectToAction("Index", "Home");
			}
			return View(model);
		}

		[Authorize(Roles = "Admin")]
		[HttpGet]
		public async Task<IActionResult> EditProduct(string productId)
		{
			var editProductVM = await _serviceManager.ProductService.GetEditProductViewModel(productId);
			editProductVM.Images = await _serviceManager.ImageService.GetImageViewModels(productId);
			return View(editProductVM);
		}

		[Authorize(Roles = "Admin")]
		[HttpPost]
		public IActionResult EditProduct(EditProductViewModel model)
		{
			if(ModelState.IsValid)
			{
				_serviceManager.ProductService.UpdateProduct(model);
				if(model.RemovedImageIds.Any() || model.NewImages.Any())
				{
					_serviceManager.ImageService.UpdateProductImages(model);
				}

				return RedirectToAction("GetProduct", "Product", new {productId = model.Id});
			}
			return View(model);
		}

		[Authorize(Roles = "Admin")]
		[HttpPost]
		public IActionResult DeleteProduct(string productId)
		{
			_serviceManager.ProductService.DeleteProduct(productId);
			return RedirectToAction("Index", "Home");
		}
	}
}
