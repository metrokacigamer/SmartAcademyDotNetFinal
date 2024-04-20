using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ShoppingApp
{
	public class ModelStateFeatureFilter : IAsyncActionFilter
	{
		public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
		{
			var state = context.ModelState;
			context.HttpContext.Features.Set<ModelStateFeature>(new ModelStateFeature(state));
			await next();
		}
	}

    public class ModelStateFeature
    {
        public ModelStateDictionary ModelState { get; set; }
        public ModelStateFeature(ModelStateDictionary state)
        {
            this.ModelState = state;
        }
    }
}
