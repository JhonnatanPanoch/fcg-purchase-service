using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;

namespace Fcg.Games.Purchase.Api.ApiConfigurations;
public static class RegisterValidations
{
    public static IServiceCollection AddAbstractValidations(this IServiceCollection services)
    {
        //services.AddValidatorsFromAssembly(typeof(AtualizarContaDtoValidator).Assembly);
        
        services.AddFluentValidationAutoValidation(options =>
        {
            options.OverrideDefaultResultFactoryWith<CustomValidatorResult>();
        });
        
        return services;
    }
}