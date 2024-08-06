namespace BlogIdentityApi.Extensions.ServiceCollectionExtensions;

using System.Reflection;

public static class AddMediatRMethod
{
    public static void AddMediatR(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddMediatR(configuration => {
            configuration.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });
    } 
}
