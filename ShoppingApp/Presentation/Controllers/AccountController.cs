using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Shared.Models;
using Service.Abstactions;
using Microsoft.AspNetCore.Identity;
using System.Text.Json;

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
		public IActionResult Login()//tested
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Login(LoginViewModel model)//tested
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
		public async Task<IActionResult> Register(RegisterViewModel model)//tested
		{
			if(ModelState.IsValid)
			{
				if(model.Password != model.ConfirmPassword)
				{
					ModelState.AddModelError("", "Passwords do not match");

                    return View(model);
                }

                var confirmationModel = await _serviceManager.EmailSenderService.SendRegisterConfirmationEmail(model);
				var modelJson = JsonSerializer.Serialize(confirmationModel);
				
				return RedirectToAction("EmailValidation", "Account", new { modelJson = new string(modelJson)});
			}

			return View(model);
		}

		[HttpGet]
		public IActionResult EmailValidation(string modelJson)//tested
		{
			var model = JsonSerializer.Deserialize<EmailConfirmationViewModel<RegisterViewModel>>(modelJson);
			return View(model);
		}

		[HttpPost]
		public async Task<IActionResult> EmailValidation(EmailConfirmationViewModel<RegisterViewModel> model)//tested
		{
			if(ModelState.IsValid)
			{
				if(model.EnteredKey == model.Key)
				{
					await _serviceManager.AccountService.RegisterAsync(model.Model);
				}
				else
				{
					ModelState.AddModelError(string.Empty, "Entered key is invalid.");

					return View(model);
				}
			}

			return RedirectToAction("Index", "Home");
		}

		[NonAction]
		public void AddErrors(IEnumerable<string> errors)//tested
		{
			foreach (var error in errors)
			{
				ModelState.AddModelError(string.Empty, error);
			}
		}

		[Authorize]
		[HttpGet]
		public async Task<IActionResult> GetProfileSettings(string actionName = "ChangeUsername")//tested
		{
			var userId = _serviceManager.AccountService.GetCurrentUserId(User);
			var userSettingsViewModel = await _serviceManager.AccountService.GetUserSettingsViewModel(userId, actionName);

			return View(userSettingsViewModel);
		}

		[Authorize]
		[HttpGet]
		public async Task<IActionResult> ChangeUsername(string userId)//tested
		{
			var user = await _serviceManager.AccountService.GetById(userId);

			return View(new ChangeUserNameViewModel
			{
				Id = userId,
				Email = user.Email,
			});
		}

		[Authorize]
		[HttpPost]
		public async Task<IActionResult> ChangeUsername(ChangeUserNameViewModel model)//tested
		{
			if (ModelState.IsValid)
			{
				var nameExists = await _serviceManager.AccountService.CheckUserName(model.NewUserName);
				if (!nameExists)
				{
					var confirmationModel = await _serviceManager.EmailSenderService.SendNameChangeConfirmationEmail(model);
					var modelJson = JsonSerializer.Serialize(confirmationModel);

					return RedirectToAction("NameChangeValidation", "Account", new {modelJson = new string(modelJson) });
				}
				else
				{
					ModelState.AddModelError(string.Empty, "Name already taken");
				}
			}

			return View(model);
		}

		[Authorize]
		[HttpGet]
		public IActionResult NameChangeValidation(string modelJson)//tested
		{
			var model = JsonSerializer.Deserialize<EmailConfirmationViewModel<ChangeUserNameViewModel>>(modelJson);

			return View(model);
		}

		[Authorize]
		[HttpPost]
		public async Task<IActionResult> NameChangeValidation(EmailConfirmationViewModel<ChangeUserNameViewModel> model)//tested
		{
			if (ModelState.IsValid)
			{
				if (model.Key == model.EnteredKey)
				{
					var result = await _serviceManager.AccountService.UpdateUsername(model.Model);

					if (!result.Succeeded)
					{
						AddErrors(result.Errors.Select(x => x.Description));

						return View(model);
					}

					return RedirectToAction("GetProfileSettings", "Account");
				}

				ModelState.AddModelError(string.Empty, "Invalid key.");

				return View(model);
			}

			return View(model);
		}

		[Authorize]
		[HttpGet]
		public IActionResult ChangePassword(string userId)//tested
		{
			return View(new ChangePasswordViewModel
			{
				Id = userId,
			});
		}

		[Authorize]
		[HttpPost]
		public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)//tested
		{
			if (ModelState.IsValid)
			{
				if(model.NewPassword == model.ConfirmNewPassword)
				{
					var res = await _serviceManager.AccountService.UpdatePassword(model);
					if (res.Succeeded)
					{
						return RedirectToAction("GetProfileSettings", "Account");
					}
					else
					{
						AddErrors(res.Errors.Select(x => x.Description));

						return View(model);
					}
				}

				ModelState.AddModelError(string.Empty, "Passwords do not match");
			}

			return View(model);
		}


		[Authorize]
		[HttpGet]
		public IActionResult ChangeEmail(string userId)//tested
		{
			return View(new ChangeEmailViewModel
			{
				Id = userId,
			});
		}

		[Authorize]
		[HttpPost]
		public async Task<IActionResult> ChangeEmail(ChangeEmailViewModel model)//tested
		{
			if (ModelState.IsValid)
			{
				var user = await _serviceManager.AccountService.GetById(model.Id);
				if(user.Email != model.Email)
				{
					ModelState.AddModelError(string.Empty, "Incorrect Email.");

					return View(model);
				}

				if(model.NewEmail == model.ConfirmNewEmail)
				{
					var emailTaken = await _serviceManager.AccountService.CheckEmail(model.NewEmail);

					if (!emailTaken)
					{
						var confirmationModel = await _serviceManager.EmailSenderService.SendEmailRemovalConfirmationEmail(model);
						var modelJson = JsonSerializer.Serialize(confirmationModel);

						return RedirectToAction("EmailRemovalValidation", "Account", new { modelJson = new string(modelJson) });
					}
					else
					{
						ModelState.AddModelError(string.Empty, "Email already taken.");

						return View(model);
					}
				}
				ModelState.AddModelError(string.Empty, "Emails do not match.");
			}

			return View(model);
		}

		[Authorize]
		[HttpGet]
		public IActionResult EmailRemovalValidation(string modelJson)//tested
		{
			var model = JsonSerializer.Deserialize<EmailConfirmationViewModel<ChangeEmailViewModel>>(modelJson);

			return View(model);
		}

		[Authorize]
		[HttpPost]
		public async Task<IActionResult> EmailRemovalValidation(EmailConfirmationViewModel<ChangeEmailViewModel> model)//tested
		{
			if(model.Key == model.EnteredKey)
			{
				var confirmationModel = await _serviceManager.EmailSenderService.SendEmailChangeConfirmationEmail(model.Model);
				var modelJson = JsonSerializer.Serialize(confirmationModel);

				return RedirectToAction("EmailChangeValidation", "Account", new { modelJson = new string(modelJson) });
			}

			ModelState.AddModelError(string.Empty, "Invalid key.");
			return View(model);
		}

		[Authorize]
		[HttpGet]
		public IActionResult EmailChangeValidation(string modelJson)//tested
		{
			var model = JsonSerializer.Deserialize<EmailConfirmationViewModel<ChangeEmailViewModel>>(modelJson);

			return View(model);
		}

		[Authorize]
		[HttpPost]
		public async Task<IActionResult> EmailChangeValidation(EmailConfirmationViewModel<ChangeEmailViewModel> model)//tested
		{
			if (model.Key == model.EnteredKey)
			{
				var res = await _serviceManager.AccountService.UpdateEmail(model);
				if (res.Succeeded)
				{
					return RedirectToAction("GetProfileSettings", "Account");
				}
				else
				{
					AddErrors(res.Errors.Select(x => x.Description));

					return View(model);
				}
			}
			ModelState.AddModelError(string.Empty, "Invalid key.");
			return View(model);
		}

		[Authorize]
		[HttpGet]
		public async Task<IActionResult> LogOut()//tested
		{
			await _serviceManager.AccountService.LogOut();

			return RedirectToAction("Index", "Home");
		}
	}
}
