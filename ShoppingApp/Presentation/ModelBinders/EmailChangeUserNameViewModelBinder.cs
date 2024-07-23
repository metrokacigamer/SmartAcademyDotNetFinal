using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Shared.Models;

public class EmailChangeUserNameViewModelBinder : IModelBinder
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
			var model = new EmailConfirmationViewModel<ChangeUserNameViewModel>();

			// Bind the top-level properties
			model.Key = bindingContext.ValueProvider.GetValue("Key").FirstValue;
			model.EnteredKey = bindingContext.ValueProvider.GetValue("EnteredKey").FirstValue;

			// Bind the nested properties
			model.Model = new ChangeUserNameViewModel
			{
				Email = bindingContext.ValueProvider.GetValue("Model.Email").FirstValue,
				Id = bindingContext.ValueProvider.GetValue("Model.Id").FirstValue,
				NewUserName = bindingContext.ValueProvider.GetValue("Model.NewUserName").FirstValue
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