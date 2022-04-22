using System.Collections;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace RentMovie.Web.Filters;

public class ModelStateAsyncFilter : IAsyncActionFilter
{
    private readonly IValidatorFactory _validatorFactory;

    public ModelStateAsyncFilter(
        IValidatorFactory validatorFactory)
    {
        _validatorFactory = validatorFactory;
    }

    /// <summary>
    ///     Validates values before the controller's action is invoked (before the route is executed).
    /// </summary>
    public async Task OnActionExecutionAsync(ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        await ValidateActionArgumentsAsync(context);

        if (!context.ModelState.IsValid)
        {
            context.Result = new BadRequestObjectResult(context.ModelState);
            return;
        }

        await next();
    }

    private async Task ValidateActionArgumentsAsync(ActionExecutingContext context)
    {
        foreach (var (_, value) in context.ActionArguments)
        {
            if (value is null)
            {
                continue;
            }

            await ValidateAsync(value, context.ModelState);

            // if an error is found or the type it not enumerable, short circuit the loop
            if (!context.ModelState.IsValid || !TypeIsEnumerable(value.GetType()))
            {
                continue;
            }

            await ValidateEnumerableObjectsAsync(value, context.ModelState);
        }
    }

    private async Task ValidateEnumerableObjectsAsync(object value, ModelStateDictionary modelState)
    {
        var underlyingType = value.GetType().GenericTypeArguments[0];
        var validator = _validatorFactory.GetValidator(underlyingType);

        if (validator == null)
        {
            return;
        }

        foreach (var item in (IEnumerable)value)
        {
            if (item is null)
            {
                continue;
            }

            var context = new ValidationContext<object>(item);
            var result = await validator.ValidateAsync(context);

            result.AddToModelState(modelState, string.Empty);
        }
    }

    private async Task ValidateAsync(object value, ModelStateDictionary modelState)
    {
        var validator = _validatorFactory.GetValidator(value.GetType());

        if (validator == null)
        {
            return;
        }

        var context = new ValidationContext<object>(value);
        var result = await validator.ValidateAsync(context);
        result.AddToModelState(modelState, string.Empty);
    }

    private static bool TypeIsEnumerable(Type type)
    {
        return type.IsGenericType && typeof(IEnumerable).IsAssignableFrom(type);
    }
}
