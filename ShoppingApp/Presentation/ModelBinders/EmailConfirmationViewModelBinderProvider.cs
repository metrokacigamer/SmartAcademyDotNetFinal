using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Presentation.ModelBinder;
using Presentation.ModelBinders;
using Shared.Models;

public class EmailConfirmationViewModelBinderProvider : IModelBinderProvider
{
	public IModelBinder GetBinder(ModelBinderProviderContext context)
	{
		if (context == null)
		{
			throw new ArgumentNullException(nameof(context));
		}

		if (context.Metadata.ModelType == typeof(EmailConfirmationViewModel<ChangeUserNameViewModel>))
		{
			return new EmailChangeUserNameViewModelBinder();
		}

		if (context.Metadata.ModelType == typeof(EmailConfirmationViewModel<ChangeEmailViewModel>))
		{
			return new EmailChangeEmailViewModelBinder();
		}

		if (context.Metadata.ModelType == typeof(EmailConfirmationViewModel<RegisterViewModel>))
		{
			return new EmailRegisterViewModelBinder();
		}

		return null;
	}
}