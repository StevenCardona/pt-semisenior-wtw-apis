using Repository.Users;
using Services.Users.Shared;

namespace Services.Users.GetUsers;

public class GetUsersService
{
    private readonly IUserRepository _userRepository;

    public GetUsersService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<IReadOnlyList<UserDto>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var users = await _userRepository.GetAll(cancellationToken);

        var result = users.Select(u => new UserDto
        {
            Id = u.Id,
            Name = u.Name,
            Mail = u.Mail,
            Rol = u.Rol,
            CreatedBy = u.CreatedBy,
            CreatedDate = u.CreatedDate,
            UpdatedBy = u.UpdatedBy,
            UpdatedDate = u.UpdatedDate
        }).ToList();

        return result;
    }
}
