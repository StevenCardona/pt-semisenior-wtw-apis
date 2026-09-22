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
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<TaskItem>> GetAll(
        string? orderBy = null,
        CancellationToken cancellationToken = default)
    {
        IQueryable<TaskItem> query = _dbContext.Tasks
            .AsNoTracking()
            .Include(t => t.User);

        if (orderBy != null && orderBy.ToLower() == "status")
        {
            query = query.OrderBy(t => t.Status).ThenByDescending(t => t.CreatedDate);
        }
        else
        {
            query = query.OrderByDescending(t => t.CreatedDate);
        }

        var result = await query.ToListAsync(cancellationToken);
        return result;
    }

    public async Task<IReadOnlyList<TaskItem>> GetByUser(
        int userId,
        TaskItemStatus? status = null,
        string? orderBy = null,
        CancellationToken cancellationToken = default)
    {
        IQueryable<TaskItem> query = _dbContext.Tasks
            .AsNoTracking()
            .Include(t => t.User)
            .Where(t => t.UserId == userId);

        if (status.HasValue)
        {
            query = query.Where(t => t.Status == status.Value);
        }

        if (orderBy != null && orderBy.ToLower() == "status")
        {
            query = query.OrderBy(t => t.Status).ThenByDescending(t => t.CreatedDate);
        }
        else
        {
            query = query.OrderByDescending(t => t.CreatedDate);
        }

        var result = await query.ToListAsync(cancellationToken);
        return result;
    }

    public async Task Update(TaskItem task, CancellationToken cancellationToken = default)
    {
        _dbContext.Tasks.Update(task);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
