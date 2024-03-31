using Shared.Models;

namespace Service.Abstactions
{
	public interface IEmailSenderService
	{
		Task SendEmailAsync(string email, string subject, string body);
		EmailConfirmationViewModel<ChangeEmailViewModel> SendEmailRemovalConfirmationEmail(ChangeEmailViewModel model);
		EmailConfirmationViewModel<ChangeEmailViewModel> SendEmailChangeConfirmationEmail(ChangeEmailViewModel model);
		EmailConfirmationViewModel<ChangeUserNameViewModel> SendNameChangeConfirmationEmail(ChangeUserNameViewModel model);
		EmailConfirmationViewModel<RegisterViewModel> SendRegisterConfirmationEmail(RegisterViewModel model);
	}
}
