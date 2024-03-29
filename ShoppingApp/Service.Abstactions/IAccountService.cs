using Microsoft.AspNetCore.Identity;
using Shared.Models;

namespace Service.Abstactions
{
	public interface IAccountService
	{
		Task SignInAsync(LoginViewModel model);
		Task<IdentityResult> RegisterAsync(RegisterViewModel model);
		Task<UserSettingsViewModel> GetUserSettingsViewModel(string userId);
	}
}
