using Microsoft.AspNetCore.Mvc;

namespace ShoppingApp.Controllers
{
	public class ProductController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
