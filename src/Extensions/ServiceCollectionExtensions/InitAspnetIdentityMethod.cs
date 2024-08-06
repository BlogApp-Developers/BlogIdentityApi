namespace BlogIdentityApi.Extensions.ServiceCollectionExtensions;

using BlogIdentityApi.Data;
using BlogIdentityApi.Role.Models;
using BlogIdentityApi.User.Models;
using Microsoft.EntityFrameworkCore;

public static class InitAspnetIdentityMethod
{
    public static void InitAspnetIdentity(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddDbContext<BlogIdentityDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("PostgreSqlDev");
            options.UseNpgsql(connectionString);
        });

        serviceCollection.AddIdentity<User, Role>( (options) => {
            options.User.RequireUniqueEmail = true;
        })
            .AddEntityFrameworkStores<BlogIdentityDbContext>();
    }
}
