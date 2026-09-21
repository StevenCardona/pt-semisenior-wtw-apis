using Models.Users;

namespace Models.Tasks;

public class TaskItem
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TaskItemStatus Status { get; set; }
    public int UserId { get; set; }
    public int CreatedBy { get; set; }
    public DateTime CreatedDate { get; set; }
    public int? UpdatedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }

    public User User { get; set; } = null!;

    public void ChangeStatus(TaskItemStatus newStatus)
    {
        if (!TaskItemStatusTransitions.CanTransition(Status, newStatus))
        {
            throw new InvalidOperationException(
                $"No se puede cambiar el estado de '{Status}' a '{newStatus}'.");
        }

        Status = newStatus;
    }
}
