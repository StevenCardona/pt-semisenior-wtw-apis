using Microsoft.EntityFrameworkCore;
using Models.Users;
using Repository.Data;

namespace Repository.Users;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _dbContext;

    public UserRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<User>> GetAll(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .AsNoTracking()
            .OrderBy(u => u.CreatedDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<User?> GetById(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<User?> GetByEmail(string mail, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Mail == mail, cancellationToken);
    }

    public async Task<User> Add(User user, CancellationToken cancellationToken = default)
    {
        _dbContext.Users.Add(user);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return user;
    }
}