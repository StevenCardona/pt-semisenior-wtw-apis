using Common.Exceptions;
using Models.Users;

namespace Models.Tasks;

public class TaskItem
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TaskItemStatus Status { get; set; }
    public int UserId { get; set; }
    /// <summary>
    /// JSON adicional (prioridad, dueDate, tags, metadata). Persistido como NVARCHAR(MAX).
    /// </summary>
    public string? AdditionalInfo { get; set; }
    public int CreatedBy { get; set; }
    public DateTime CreatedDate { get; set; }
    public int? UpdatedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }

    public User User { get; set; } = null!;

    public void ChangeStatus(TaskItemStatus newStatus)
    {
        if (Status == TaskItemStatus.Pending && newStatus == TaskItemStatus.InProgress)
        {
            Status = newStatus;
            return;
        }

        if (Status == TaskItemStatus.InProgress && newStatus == TaskItemStatus.Done)
        {
            Status = newStatus;
            return;
        }

        throw new BadRequestException(
            $"No se puede cambiar el estado de '{Status}' a '{newStatus}'.");
    }
}
