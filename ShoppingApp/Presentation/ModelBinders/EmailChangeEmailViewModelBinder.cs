using Microsoft.AspNetCore.Mvc.ModelBinding;
using Shared.Models;

namespace Presentation.ModelBinder
{
	public class EmailChangeEmailViewModelBinder : IModelBinder
	{
		public Task BindModelAsync(ModelBindingContext bindingContext)
		{
			if (bindingContext == null)
			{
				throw new ArgumentNullException(nameof(bindingContext));
			}

			try
			{
				// Create an instance of EmailConfirmationViewModel
				var model = new EmailConfirmationViewModel<ChangeEmailViewModel>();

				// Bind the top-level properties
				model.Key = bindingContext.ValueProvider.GetValue("Key").FirstValue;
				model.EnteredKey = bindingContext.ValueProvider.GetValue("EnteredKey").FirstValue;

				// Bind the nested properties
				model.Model = new ChangeEmailViewModel
				{
					Email = bindingContext.ValueProvider.GetValue("Model.Email").FirstValue,
					Id = bindingContext.ValueProvider.GetValue("Model.Id").FirstValue,
					NewEmail = bindingContext.ValueProvider.GetValue("Model.NewEmail").FirstValue,
					ConfirmNewEmail = bindingContext.ValueProvider.GetValue("Model.ConfirmNewEmail").FirstValue
				};

				// Set the model
				bindingContext.Result = ModelBindingResult.Success(model);
			}
			catch
			{
				bindingContext.Result = ModelBindingResult.Failed();
			}

			return Task.CompletedTask;
		}
	}
}
