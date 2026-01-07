using Fcg.Games.Purchase.Domain.Exceptions.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace Fcg.Games.Purchase.Api.Filters;

public class ValidateGuidQueryParamsFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        foreach (var param in context.ActionArguments)
        {
            if (param.Value is Guid guid && guid == Guid.Empty)
            {
                var response = new ErrorResponse(
                    (int)HttpStatusCode.BadRequest,
                    "Erros de validação",
                    new Dictionary<string, string[]>() { { $"{param.Key}", ["O parâmetro informado não pode ser vazio."] } });

                context.Result = new BadRequestObjectResult(response);

                return;
            }
        }
    }

    public void OnActionExecuted(ActionExecutedContext context) { }
}
