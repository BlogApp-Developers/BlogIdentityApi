namespace BlogIdentityApi.Extensions.ServiceCollectionExtensions;

public static class InitCorsMethod
{
    public static void InitCors(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddCors(options =>
        {
            options.AddPolicy("LocalHostPolicy", policyBuilder =>
            {
                policyBuilder
                    .WithOrigins("http://localhost")
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });

            options.AddPolicy("BlazorTestPolicy", policyBuilder =>
            {
                policyBuilder
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });
    }
}
