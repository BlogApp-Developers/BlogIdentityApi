namespace BlogIdentityApi.Extensions.ServiceCollectionExtensions;

using BlogIdentityApi.Validators;
using FluentValidation;

public static class AddValidatorsMethod
{
    public static void AddValidators(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddValidatorsFromAssemblyContaining<UserRegistrationValidator>();
        serviceCollection.AddValidatorsFromAssemblyContaining<UserLoginValidator>();
    }
}
