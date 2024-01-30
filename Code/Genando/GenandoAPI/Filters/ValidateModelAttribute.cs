using Entities.Abstract;
using FluentValidation;
using Microsoft.AspNetCore.Mvc.Filters;

namespace GenandoAPI.Filters;

public class ValidateModelAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        foreach (var actionArgument in context.ActionArguments)
        {
            //validate that model is having validator and resolve it
            if (actionArgument.Value is IBaseValidationModel model)
            {
                var modelType = actionArgument.Value.GetType();
                var genericType = typeof(IValidator<>).MakeGenericType(modelType);
                var validator = context.HttpContext.RequestServices.GetService(genericType);

                if (validator != null)
                {
                    model.Validate(validator, model);
                }
            }
        }

        base.OnActionExecuting(context);
    }
}
