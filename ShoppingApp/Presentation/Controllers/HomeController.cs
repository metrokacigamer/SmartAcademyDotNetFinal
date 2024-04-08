using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Shared.Models;
using Service.Abstactions;
using System.Text.Json;

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

		[HttpGet]
		public async Task<IActionResult> Index(string searchString = "", int currentPage = 0, int pageSize = 5, string sortBy = "Price", bool ascending = false)
		{
			var productVMs = await _serviceManager.ProductService.GetProductsViewModels(searchString, currentPage, pageSize);
			productVMs = await _serviceManager.ProductService.SortBy(productVMs, sortBy, ascending);

			foreach (var productVM in productVMs)
			{
				productVM.Images = await _serviceManager.ImageService.GetImageViewModels(productVM.Id);
			}
			ViewData["CurrentPage"] = currentPage;
			ViewData["TotalPages"] = await _serviceManager.ProductService.GetTotalPages(new FilterViewModel
			{
				SearchString = searchString,
			}, pageSize);
			ViewData["SearchString"] = searchString;
			ViewData["SortBy"] = sortBy;
			ViewData["Ascending"] = ascending;

			return View(productVMs);
		}

		[HttpGet]
		public IActionResult Filter()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Filter(FilterViewModel model, int currentPage = 0, int pageSize = 5)
		{
			var filterModel = await _serviceManager.ProductService.Filter(model, currentPage, pageSize);
			filterModel.ProductViewModels = await _serviceManager.ProductService.SortBy(
														filterModel.ProductViewModels,
														filterModel.Filter.SortBy,
														filterModel.Filter.Ascending);
			var modelJson = JsonSerializer.Serialize(filterModel);

			return RedirectToAction("FilteredPage", "Home", new { modelJson = new string(modelJson), currentPage = currentPage, pageSize = pageSize });
		}

		[HttpGet]
		public async Task<IActionResult> FilteredPage(string modelJson, int currentPage, int pageSize)
		{
			var model = JsonSerializer.Deserialize<FilteredPageViewModel>(modelJson);

			ViewData["CurrentPage"] = currentPage;
			ViewData["TotalPages"] = await _serviceManager.ProductService.GetTotalPages(model.Filter, pageSize);
			ViewData["PageSize"] = pageSize;
			return View(model);
		}

		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error(Exception ex)
		{
			return View(new ErrorViewModel 
			{
				RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
				ErrorMessage = ex.Message,
			});
		}
	}
}
