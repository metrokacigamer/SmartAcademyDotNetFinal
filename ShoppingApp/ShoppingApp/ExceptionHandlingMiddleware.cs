using Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Presentation.Controllers;
using Shared.Models;

namespace ShoppingApp
{
	public class ExceptionHandlingMiddleware : IMiddleware
	{
		private readonly IServiceProvider _serviceProvider;

		public ExceptionHandlingMiddleware(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
		{
			try
			{
				await next(context);
			}
			catch(Exception ex)
			{
				await HandleExceptionAsync(context, ex);
			}
		}

		private async Task HandleExceptionAsync(HttpContext context, Exception ex)
		{
			switch (ex)
			{
				case ProductExistsException:
					{
						var ModelState = context.Features.Get<ModelStateFeature>()?.ModelState;
						ModelState.AddModelError("", "Product with the same name already exists.");

						context.Response.Redirect("/Product/AddProduct");
						break;
					}
				case RegisterFailedException:
					{
						var _ex = ex as RegisterFailedException;
						var ModelState = context.Features.Get<ModelStateFeature>()?.ModelState;

						foreach(var error in _ex.Result.Errors)
						{
							ModelState.AddModelError(string.Empty, error.Description);
						}
						context.Response.Redirect("/Account/Register");

						break;
					}
				case UserNotFoundException _ex:
					{
                        var ModelState = context.Features.Get<ModelStateFeature>()?.ModelState;

                        ModelState.AddModelError(string.Empty, _ex.Message);

                        context.Response.Redirect("/Account/Login");
                        break;
					}
				case LoginFailedException:
					{
						var _ex = ex as LoginFailedException;
						var ModelState = context.Features.Get<ModelStateFeature>()?.ModelState;

						ModelState.AddModelError(string.Empty, _ex.Message);

						context.Response.Redirect("/Account/Login");
						break;
					}
				default:
					{
						context.Response.Redirect("/Home/Error");

						break;
					}
			}
		}
	}
}
