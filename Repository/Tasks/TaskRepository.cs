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
        CancellationToken cancellationToken = default)
    {
        IQueryable<TaskItem> query = _dbContext.Tasks
            .AsNoTracking()
            .Include(task => task.User);

        query = ApplyOrder(query, orderBy);

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TaskItem>> GetByUser(
        int userId,
        TaskItemStatus? status = null,
        string? orderBy = null,
        CancellationToken cancellationToken = default)
    {
        IQueryable<TaskItem> query = _dbContext.Tasks
            .AsNoTracking()
            .Include(task => task.User)
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

    private static IQueryable<TaskItem> ApplyOrder(IQueryable<TaskItem> query, string? orderBy)
    {
        if (orderBy != null && orderBy.Equals("status", StringComparison.OrdinalIgnoreCase))
        {
            return query.OrderBy(task => task.Status).ThenByDescending(task => task.CreatedDate);
        }

        return query.OrderByDescending(task => task.CreatedDate);
    }
}
