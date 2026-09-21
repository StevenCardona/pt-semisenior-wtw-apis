using Common.Exceptions;
using Models.Users;
using Repository.Users;
using Services.Users.Shared;

namespace Services.Users.CreateUser;

public class CreateUserService
{
    private readonly IUserRepository _userRepository;

    public CreateUserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserDto> ExecuteAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        var mail = request.Mail.Trim();

        var existing = await _userRepository.GetByEmail(mail, cancellationToken);

        if (existing is not null)
        {
            throw new ConflictException($"Ya existe un usuario con el correo '{mail}'.");
        }

        var user = new User
        {
            Name = request.Name.Trim(),
            Mail = mail,
            Rol = request.Rol,
            CreatedBy = request.CreatedBy,
            CreatedDate = DateTime.UtcNow,
            UpdatedBy = null,
            UpdatedDate = null
        };

        var created = await _userRepository.Add(user, cancellationToken);

        var result = new UserDto
        {
            Id = created.Id,
            Name = created.Name,
            Mail = created.Mail,
            Rol = created.Rol,
            CreatedBy = created.CreatedBy,
            CreatedDate = created.CreatedDate,
            UpdatedBy = created.UpdatedBy,
            UpdatedDate = created.UpdatedDate
        };

        return result;
    }
}
