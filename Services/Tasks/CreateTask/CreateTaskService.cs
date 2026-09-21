using Common.Exceptions;
using Models.Tasks;
using Repository.Tasks;
using Repository.Users;
using Services.Tasks.Shared;

namespace Services.Tasks.CreateTask;

public class CreateTaskService
{
    private readonly ITaskRepository _taskRepository;
    private readonly IUserRepository _userRepository;

    public CreateTaskService(ITaskRepository taskRepository, IUserRepository userRepository)
    {
        _taskRepository = taskRepository;
        _userRepository = userRepository;
    }

    public async Task<TaskDto> ExecuteAsync(CreateTaskRequest request, CancellationToken cancellationToken = default)
    {
        var name = request.Name.Trim();

        if (string.IsNullOrEmpty(name))
        {
            throw new BadRequestException("El título de la tarea es obligatorio.");
        }

        var user = await _userRepository.GetById(request.UserId, cancellationToken);

        if (user is null)
        {
            throw new NotFoundException($"No existe un usuario con el id '{request.UserId}'.");
        }

        string? description = null;

        if (!string.IsNullOrWhiteSpace(request.Description))
        {
            description = request.Description.Trim();
        }

        var task = new TaskItem
        {
            Name = name,
            Description = description,
            Status = TaskItemStatus.Pending,
            UserId = request.UserId,
            CreatedBy = request.CreatedBy,
            CreatedDate = DateTime.UtcNow,
            UpdatedBy = null,
            UpdatedDate = null
        };

        var created = await _taskRepository.Add(task, cancellationToken);

        var result = new TaskDto
        {
            Id = created.Id,
            Name = created.Name,
            Description = created.Description,
            Status = created.Status,
            UserId = created.UserId,
            CreatedBy = created.CreatedBy,
            CreatedDate = created.CreatedDate,
            UpdatedBy = created.UpdatedBy,
            UpdatedDate = created.UpdatedDate
        };

        return result;
    }
}
