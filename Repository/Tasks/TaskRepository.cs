using Microsoft.EntityFrameworkCore;
using Models.Tasks;
using Repository.Data;

namespace Repository.Tasks;

public class TaskRepository : ITaskRepository
{
    private readonly AppDbContext _dbContext;

    public TaskRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<TaskItem> Add(TaskItem task, CancellationToken cancellationToken = default)
    {
        _dbContext.Tasks.Add(task);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return task;
    }

    public async Task<TaskItem?> GetById(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Tasks
            .Include(task => task.User)
            .FirstOrDefaultAsync(task => task.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<TaskItem>> GetAll(
        string? orderBy = null,
        string? priority = null,
        CancellationToken cancellationToken = default)
    {
        IQueryable<TaskItem> query = BuildBaseQuery(priority);

        query = ApplyOrder(query, orderBy);

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TaskItem>> GetByUser(
        int userId,
        TaskItemStatus? status = null,
        string? orderBy = null,
        string? priority = null,
        CancellationToken cancellationToken = default)
    {
        IQueryable<TaskItem> query = BuildBaseQuery(priority)
            .Where(task => task.UserId == userId);

        if (status.HasValue)
        {
            query = query.Where(task => task.Status == status.Value);
        }

        query = ApplyOrder(query, orderBy);

        return await query.ToListAsync(cancellationToken);
    }

    public async Task Update(TaskItem task, CancellationToken cancellationToken = default)
    {
        _dbContext.Tasks.Update(task);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> PatchPriority(
        int taskId,
        string priority,
        int updatedBy,
        CancellationToken cancellationToken = default)
    {
        var exists = await _dbContext.Tasks
            .AnyAsync(task => task.Id == taskId, cancellationToken);

        if (!exists)
        {
            return false;
        }

        // JSON_MODIFY: actualiza $.priority; si AdditionalInfo es NULL, crea el objeto base.
        var rows = await _dbContext.Database.ExecuteSqlInterpolatedAsync($@"
UPDATE [Tasks]
SET [AdditionalInfo] = JSON_MODIFY(
        COALESCE([AdditionalInfo], N'{{}}'),
        '$.priority',
        {priority}),
    [UpdatedBy] = {updatedBy},
    [UpdatedDate] = {DateTime.UtcNow}
WHERE [Id] = {taskId};
", cancellationToken);

        return rows > 0;
    }

    /// <summary>
    /// Usa JSON_VALUE nativo de SQL Server cuando hay filtro de prioridad.
    /// </summary>
    private IQueryable<TaskItem> BuildBaseQuery(string? priority)
    {
        IQueryable<TaskItem> query;

        if (!string.IsNullOrWhiteSpace(priority))
        {
            query = _dbContext.Tasks.FromSqlInterpolated($@"
                SELECT *
                FROM [Tasks]
                WHERE JSON_VALUE([AdditionalInfo], '$.priority') = {priority}");
        }
        else
        {
            query = _dbContext.Tasks;
        }

        return query
            .AsNoTracking()
            .Include(task => task.User);
    }

    private static IQueryable<TaskItem> ApplyOrder(IQueryable<TaskItem> query, string? orderBy)
    {
        if (orderBy != null && orderBy.Equals("status", StringComparison.OrdinalIgnoreCase))
        {
            return query.OrderBy(task => task.Status).ThenByDescending(task => task.CreatedDate);
        }

        return query.OrderByDescending(task => task.CreatedDate);
    }
}
