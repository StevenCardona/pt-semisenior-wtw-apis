using Models.Tasks;
using Models.Users;

namespace Services.Tasks.Shared;

public static class TaskDtoMapper
{
    public static TaskDto ToDto(TaskItem task)
    {
        return ToDto(task, task.User);
    }

    public static TaskDto ToDto(TaskItem task, User assignedUser)
    {
        return new TaskDto
        {
            Id = task.Id,
            Name = task.Name,
            Description = task.Description,
            Status = task.Status,
            AssignedTo = ToAssignedTo(assignedUser),
            AdditionalInfo = TaskAdditionalInfoSerializer.Deserialize(task.AdditionalInfo),
            CreatedBy = task.CreatedBy,
            CreatedDate = task.CreatedDate,
            UpdatedBy = task.UpdatedBy,
            UpdatedDate = task.UpdatedDate
        };
    }

    private static TaskAssignedToDto ToAssignedTo(User user)
    {
        return new TaskAssignedToDto
        {
            Id = user.Id,
            Name = user.Name,
            Mail = user.Mail,
            Rol = user.Rol
        };
    }
}
