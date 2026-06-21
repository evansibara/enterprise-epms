using EPMS.Application.Common;
using FluentValidation;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EPMS.WebApi.Filters;

/// <summary>
/// Menjalankan FluentValidation secara otomatis terhadap setiap parameter
/// action yang punya IValidator&lt;T&gt; terdaftar di DI (lihat
/// ApplicationServiceRegistration.AddValidatorsFromAssemblyContaining),
/// sebelum action method benar-benar dieksekusi. Tanpa filter ini, validator
/// yang sudah ditulis di EPMS.Application/Validators tidak akan pernah
/// dipanggil oleh siapa pun.
/// </summary>
public class ValidationActionFilter : IAsyncActionFilter
{
    private readonly IServiceProvider _serviceProvider;

    public ValidationActionFilter(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        foreach (var argument in context.ActionArguments.Values)
        {
            if (argument is null)
            {
                continue;
            }

            var argumentType = argument.GetType();
            var validatorType = typeof(IValidator<>).MakeGenericType(argumentType);

            if (_serviceProvider.GetService(validatorType) is not IValidator validator)
            {
                continue;
            }

            var validationContext = new ValidationContext<object>(argument);
            var result = await validator.ValidateAsync(validationContext, context.HttpContext.RequestAborted);

            if (!result.IsValid)
            {
                var errors = result.Errors.Select(e => e.ErrorMessage).ToList();
                context.Result = new Microsoft.AspNetCore.Mvc.BadRequestObjectResult(
                    ApiResponse.Fail("Validasi gagal.", errors));
                return;
            }
        }

        await next();
    }
}
