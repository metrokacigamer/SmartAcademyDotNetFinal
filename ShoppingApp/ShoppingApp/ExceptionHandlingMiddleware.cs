
using ShoppingApp.Controllers;

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
				default:
					{
						_serviceProvider.GetService<HomeController>().Error();
						break;
					}
			}
		}
	}
}
