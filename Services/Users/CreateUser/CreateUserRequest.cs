using System.ComponentModel.DataAnnotations;
using Models.Users;

namespace Services.Users.CreateUser;

public class CreateUserRequest
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [MaxLength(200, ErrorMessage = "El nombre no puede superar los 200 caracteres.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "El correo es obligatorio.")]
    [EmailAddress(ErrorMessage = "El correo no tiene un formato válido.")]
    [MaxLength(256, ErrorMessage = "El correo no puede superar los 256 caracteres.")]
    public string Mail { get; set; } = string.Empty;

    [Required(ErrorMessage = "El rol es obligatorio.")]
    [EnumDataType(typeof(UserRole), ErrorMessage = "El rol debe ser 'admin' o 'user'.")]
    public UserRole Rol { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "CreatedBy debe ser mayor que 0.")]
    public int CreatedBy { get; set; }
}
