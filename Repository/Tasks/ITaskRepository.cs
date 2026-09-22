using Models.Tasks;

namespace Repository.Tasks;

public interface ITaskRepository
{
    Task<TaskItem> Add(TaskItem task, CancellationToken cancellationToken = default);
    Task<TaskItem?> GetById(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TaskItem>> GetAll(
        string? orderBy = null,
        string? priority = null,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TaskItem>> GetByUser(
        int userId,
        TaskItemStatus? status = null,
        string? orderBy = null,
        string? priority = null,
        CancellationToken cancellationToken = default);
    Task Update(TaskItem task, CancellationToken cancellationToken = default);
    Task<bool> PatchPriority(int taskId, string priority, int updatedBy, CancellationToken cancellationToken = default);
}
