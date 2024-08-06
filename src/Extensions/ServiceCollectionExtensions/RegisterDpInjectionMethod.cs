namespace BlogIdentityApi.Extensions.ServiceCollectionExtensions;

using BlogIdentityApi.RefreshToken.Repositories;
using BlogIdentityApi.RefreshToken.Repositories.Ef_Core;
using BlogIdentityApi.Verification;
using BlogIdentityApi.Verification.Base;

public static class RegisterDpInjectionMethod
{
    public static void RegisterDpInjection(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IEmailService, EmailService>();

        serviceCollection.AddScoped<IRefreshTokenRepository, RefreshTokenEfCoreRepository>();
    } 
}
