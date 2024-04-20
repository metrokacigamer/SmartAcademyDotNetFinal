using Shared.Models;

namespace Service.Abstactions
{
	public interface IEmailSenderService
	{
		Task SendEmailAsync(string email, string subject, string body);
		Task<EmailConfirmationViewModel<ChangeEmailViewModel>> SendEmailRemovalConfirmationEmail(ChangeEmailViewModel model);
		Task<EmailConfirmationViewModel<ChangeEmailViewModel>> SendEmailChangeConfirmationEmail(ChangeEmailViewModel model);
		Task<EmailConfirmationViewModel<ChangeUserNameViewModel>> SendNameChangeConfirmationEmail(ChangeUserNameViewModel model);
		Task<EmailConfirmationViewModel<RegisterViewModel>> SendRegisterConfirmationEmail(RegisterViewModel model);
	}
}
