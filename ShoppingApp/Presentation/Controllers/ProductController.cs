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

		[Authorize(Roles = "Admin")]
		[HttpGet]
		public IActionResult AddProduct()
		{
			return View();
		}

		[Authorize(Roles = "Admin")]
		[HttpPost]
		public async Task<IActionResult> AddProduct(AddProductViewModel model, params IFormFile[] images)
		{
			if(ModelState.IsValid)
			{
				var product = await _serviceManager.ProductService.AddProduct(model);
				await _serviceManager.ImageService.AddImages(model.ProductImages, product);
				return RedirectToAction("Index", "Home");
			}
			return View(model);
		}
	}
}
