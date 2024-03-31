using Microsoft.AspNetCore.Identity;
using Shared.Models;
using System.Security.Claims;
using Domain.Entities;

namespace Service.Abstactions
{
	public interface IAccountService
	{
		Task SignInAsync(LoginViewModel model);
		Task<IdentityResult> RegisterAsync(RegisterViewModel model);
		Task<UserSettingsViewModel> GetUserSettingsViewModel(string userId);
		string GetCurrentUserId(ClaimsPrincipal User);
		bool RequestIsAuthenticated(string id, ClaimsPrincipal User);
		Task<AppUser> GetById(string id);
		Task LogOut();
	}
}
