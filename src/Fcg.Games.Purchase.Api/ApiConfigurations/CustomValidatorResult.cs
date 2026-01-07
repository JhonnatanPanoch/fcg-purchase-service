using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Results;
using System.Net;
using Fcg.Games.Purchase.Domain.Exceptions.Responses;

namespace Fcg.Games.Purchase.Api.ApiConfigurations;

public class CustomValidatorResult : IFluentValidationAutoValidationResultFactory
{
    public IActionResult CreateActionResult(ActionExecutingContext context, ValidationProblemDetails? validationProblemDetails)
    {
        return new BadRequestObjectResult(
            new ErrorResponse(
                (int)HttpStatusCode.BadRequest,
                "Erros de validação",
                validationProblemDetails?.Errors ?? new Dictionary<string, string[]>()));
    }
}
