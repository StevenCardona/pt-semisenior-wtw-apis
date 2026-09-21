namespace Models.Tasks;

public enum TaskItemStatus
{
    Pending,
    InProgress,
    Done
}

public static class TaskItemStatusTransitions
{
    public static bool CanTransition(TaskItemStatus from, TaskItemStatus to) => (from, to) switch
    {
        (TaskItemStatus.Pending, TaskItemStatus.InProgress) => true,
        (TaskItemStatus.InProgress, TaskItemStatus.Done) => true,
        _ => false
    };
}
