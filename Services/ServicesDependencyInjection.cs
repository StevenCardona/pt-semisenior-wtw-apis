using Microsoft.Extensions.DependencyInjection;
using Repository.Users;
using Services.Users.CreateUser;
using Services.Users.GetUsers;

namespace Services;

public static class ServicesDependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<GetUsersService>();
        services.AddScoped<CreateUserService>();
        return services;
    }
}
