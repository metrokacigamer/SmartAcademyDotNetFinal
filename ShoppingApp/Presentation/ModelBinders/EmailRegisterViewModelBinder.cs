using Microsoft.AspNetCore.Mvc.ModelBinding;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.ModelBinders
{
	public class EmailRegisterViewModelBinder : IModelBinder
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
				var model = new EmailConfirmationViewModel<RegisterViewModel>();

				// Bind the top-level properties
				model.Key = bindingContext.ValueProvider.GetValue("Key").FirstValue;
				model.EnteredKey = bindingContext.ValueProvider.GetValue("EnteredKey").FirstValue;

				// Bind the nested properties
				model.Model = new RegisterViewModel
				{
					Email = bindingContext.ValueProvider.GetValue("Model.Email").FirstValue,
					UserName = bindingContext.ValueProvider.GetValue("Model.UserName").FirstValue,
					Password = bindingContext.ValueProvider.GetValue("Model.Password").FirstValue,
					ConfirmPassword = bindingContext.ValueProvider.GetValue("Model.ConfirmPassword").FirstValue,
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
