using Models.Users;

namespace Repository.Users;

public interface IUserRepository
{
    Task<IReadOnlyList<User>> GetAll(CancellationToken cancellationToken = default);
    Task<User?> GetById(int id, CancellationToken cancellationToken = default);
    Task<User?> GetByEmail(string mail, CancellationToken cancellationToken = default);
    Task<User> Add(User user, CancellationToken cancellationToken = default);
}
