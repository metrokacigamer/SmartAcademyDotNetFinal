﻿using Domain.Repositories;
using Service.Abstactions;
using Domain.Entities;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Domain.Exceptions;
using Shared.Models;
using System.Text.Json;

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
				var res = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);
				if (!res.Succeeded)
				{
					throw new LoginFailedException("Incorrect Password");
				}
			}
			else
			{
				throw new UserNotFoundException("No user with such username");// implement this
			}
		}

		public async Task RegisterAsync(RegisterViewModel model)
		{
			await SuchAccountExists(model);

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
				_repositoryManager.CartRepository.Create(new Cart
				{
					User = user
				});
			}
			else
			{
				throw new RegisterFailedException(result);
			}
		}

		private async Task SuchAccountExists(RegisterViewModel model)
		{
			var emailTaken = await CheckEmail(model.Email);
			var nameTaken = await CheckUserName(model.UserName);
			if (emailTaken || nameTaken)
			{
				throw new UsernameOrEmailTakenException();// implement this
			}
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