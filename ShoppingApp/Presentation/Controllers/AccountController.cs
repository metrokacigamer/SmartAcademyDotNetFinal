using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Shared.Models;
using Service.Abstactions;
using Microsoft.AspNetCore.Identity;

namespace Presentation.Controllers
{
	public class AccountController : Controller
	{
		private readonly IServiceManager _serviceManager;

		public AccountController(IServiceManager serviceManager)
		{
			_serviceManager = serviceManager;
		}

		[AllowAnonymous]
		[HttpGet]
		public IActionResult Login()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Login(LoginViewModel model)
		{
			if (ModelState.IsValid)
			{
				await _serviceManager.AccountService.SignInAsync(model);
				return RedirectToAction("Index", "Home");
			}
			return View(model);
		}

		[AllowAnonymous]
		[HttpGet]
		public IActionResult Register()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Register(RegisterViewModel model)
		{
			if(ModelState.IsValid)
			{
				var result = await _serviceManager.AccountService.RegisterAsync(model);
				if (!result.Succeeded)
				{
					AddErrors(result.Errors.Select(x => x.Description));
					return View(model);
				}
			}
			return RedirectToAction("Home", "Index");
		}

		[NonAction]
		public void AddErrors(IEnumerable<string> errors)
		{
			foreach (var error in errors)
			{
				ModelState.AddModelError(string.Empty, error);
			}
		}

		[Authorize]
		[HttpGet]
		public IActionResult GetProfileSettings(string userId)
		{
			var userSettingsViewModel = _serviceManager.AccountService.GetUserSettingsViewModel(userId);

			return View(userSettingsViewModel);
		}

		[Authorize]
		[HttpGet]
		public async Task<IActionResult> LogOut()
		{
			await _serviceManager.AccountService.LogOut();

			return RedirectToAction("Index", "Home");
		}
	}
}
