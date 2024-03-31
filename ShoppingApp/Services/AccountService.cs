using Domain.Repositories;
using Service.Abstactions;
using Domain.Entities;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Domain.Exceptions;
using Shared.Models;

namespace services
{
	internal class AccountService : IAccountService
	{
		private readonly IRepositoryManager _repositoryManager;
		private readonly UserManager<AppUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly SignInManager<AppUser> _signInManager;

		public AccountService(IRepositoryManager repositoryManager, 
							UserManager<AppUser> userManager,
							RoleManager<IdentityRole> roleManager,
							SignInManager<AppUser> signInManager)
		{
			_repositoryManager = repositoryManager;
			_userManager = userManager;
			_roleManager = roleManager;
			_signInManager = signInManager;
		}

		public async Task SignInAsync(LoginViewModel model)
		{
			var user = await _userManager.FindByNameAsync(model.UserName);
			if (user != null) 
			{
				await _signInManager.SignInAsync(user, isPersistent: false, model.UserName);
			}
			else
			{
				throw new UserNotFoundException("No user with such username");// implement this
			}
		}

		public async Task<IdentityResult> RegisterAsync(RegisterViewModel model)
		{
			var suchAccountExists = SuchAccountExists(model);
			if (!suchAccountExists)
			{
				var user = new AppUser
				{
					UserName = model.UserName,
					Email = model.Email
				};
				var result = await _userManager.CreateAsync(user, model.Password);

				if (result.Succeeded)
				{
					await _userManager.AddToRoleAsync(user, "Guest");
					await _signInManager.SignInAsync(user, false);
				}

				return result;
			}
			else
			{
				throw new UsernameOrEmailTakenException();// implement this
			}
		}

		private bool SuchAccountExists(RegisterViewModel model)
		{
			var emailTaken = _userManager.FindByEmailAsync(model.Email) != null;
			var nameTaken = _userManager.FindByNameAsync(model.UserName) != null;
			return !emailTaken && !nameTaken;
		}

		public async Task<UserSettingsViewModel> GetUserSettingsViewModel(string userId, string actionName)
		{
			var user = await _userManager.FindByIdAsync(userId);
			return new UserSettingsViewModel
			{
				UserName = user!.UserName!,
				ActionName = actionName,
				Email = user.Email!,
				Id = user.Id,
			};
		}

		public string GetCurrentUserId(ClaimsPrincipal User)
		{
			return User.FindFirst(ClaimTypes.NameIdentifier).Value;
		}

		public bool RequestIsAuthenticated(string id, ClaimsPrincipal User)
		{
			return id == GetCurrentUserId(User);
		}

		public async Task<AppUser> GetById(string id)
		{
			return await _userManager.FindByIdAsync(id);
		}

		public async Task LogOut()
		{
			await _signInManager.SignOutAsync();
		}

		public async Task<bool> CheckUserName(string newUserName)
		{
			return (await _userManager.FindByNameAsync(newUserName)) != null;
		}

		public async Task<IdentityResult> UpdateUsername(ChangeUserNameViewModel model)
		{
			var user = await _userManager.FindByIdAsync(model.Id);
			user!.UserName = model.NewUserName;

			return await _userManager.UpdateAsync(user);
		}

		public async Task<IdentityResult> UpdatePassword(ChangePasswordViewModel model)
		{
			var user = await GetById(model.Id);

			return await _userManager.ChangePasswordAsync(user, model.Password, model.NewPassword);
		}

		public async Task<IdentityResult> UpdateEmail(EmailConfirmationViewModel<ChangeEmailViewModel> model)
		{
			var user = await GetById(model.Model.Id);
			var token = await _userManager.GenerateChangeEmailTokenAsync(user, model.Model.NewEmail);

			return await _userManager.ChangeEmailAsync(user, model.Model.NewEmail, token);
		}

		public async Task<bool> CheckEmail(string newEmail)
		{
			return (await _userManager.FindByEmailAsync(newEmail)) != null;
		}
	}
}
