using Microsoft.AspNetCore.Identity;
using Shared.Models;
using System.Security.Claims;
using Domain.Entities;

namespace Service.Abstactions
{
	public interface IAccountService
	{
		Task SignInAsync(LoginViewModel model);
		Task RegisterAsync(RegisterViewModel model);
		Task<UserSettingsViewModel> GetUserSettingsViewModel(string userId, string actionName);
		string GetCurrentUserId(ClaimsPrincipal User);
		bool RequestIsAuthenticated(string id, ClaimsPrincipal User);
		Task<AppUser> GetById(string id);
		Task LogOut();
		Task<bool> CheckUserName(string newUserName);
		Task<IdentityResult> UpdateUsername(ChangeUserNameViewModel model);
		Task<IdentityResult> UpdatePassword(ChangePasswordViewModel model);
		Task<bool> CheckEmail(string newEmail);
		Task<IdentityResult> UpdateEmail(EmailConfirmationViewModel<ChangeEmailViewModel> model);
	}
}
