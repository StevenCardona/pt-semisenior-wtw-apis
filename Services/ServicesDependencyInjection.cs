using Microsoft.Extensions.DependencyInjection;
using Repository.Tasks;
using Repository.Users;
using Services.Tasks.ChangeStatus;
using Services.Tasks.CreateTask;
using Services.Tasks.GetTasks;
using Services.Tasks.GetTasksByUser;
using Services.Tasks.UpdateAdditionalInfo;
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

        services.AddScoped<ITaskRepository, TaskRepository>();
        services.AddScoped<CreateTaskService>();
        services.AddScoped<GetTasksService>();
        services.AddScoped<ChangeTaskStatusService>();
        services.AddScoped<GetTasksByUserService>();
        services.AddScoped<UpdateTaskAdditionalInfoService>();

        return services;
    }
}
