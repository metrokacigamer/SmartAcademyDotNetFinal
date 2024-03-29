using Domain.Repositories;
using Service.Abstactions;
using Domain.Entities;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Domain.Exceptions;
using Shared.Models;

namespace Services
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

		public async Task<UserSettingsViewModel> GetUserSettingsViewModel(string userId)
		{
			var user = await _userManager.FindByIdAsync(userId);
			return new UserSettingsViewModel
			{
				UserName = user!.UserName!,
				ActionName = "Change Username",
				Email = user.Email!,
				Id = user.Id,
			};
		}
	}
}
