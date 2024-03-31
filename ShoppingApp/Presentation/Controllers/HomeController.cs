using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Shared.Models;
using Service.Abstactions;

namespace Presentation.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly IServiceManager _serviceManager;

		public HomeController(ILogger<HomeController> logger, IServiceManager serviceManager)
		{
			_logger = logger;
			_serviceManager = serviceManager;
		}

		public async Task<IActionResult> Index(string searchString = "", int currentPage = 0, int pageSize = 5)
		{
			var productVMs = await _serviceManager.ProductService.GetProductsViewModels(searchString, currentPage, pageSize);
			foreach (var productVM in productVMs)
			{
				productVM.Images = await _serviceManager.ImageService.GetImageViewModels(productVM.Id);
			}
			ViewData["CurrentPage"] = currentPage;
			ViewData["PageSize"] = pageSize;

			return View(productVMs);
		}

		[HttpGet]
		public IActionResult Filter()//Category, Price range, searchString
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Filter(FilterViewModel model)
		{
			var vm = await _serviceManager.ProductService.Filter(model);
			return View(vm);
		}

		[HttpGet]
		public IActionResult FilteredPage()
		{

			return View();
		}

		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
