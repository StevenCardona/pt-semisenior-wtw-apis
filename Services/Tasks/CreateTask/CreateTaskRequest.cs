using System.ComponentModel.DataAnnotations;

namespace Services.Tasks.CreateTask;

public class CreateTaskRequest
{
    [Required(ErrorMessage = "El título de la tarea es obligatorio.")]
    [MaxLength(200, ErrorMessage = "El título no puede superar los 200 caracteres.")]
    public string Name { get; set; } = string.Empty;

    [MaxLength(2000, ErrorMessage = "La descripción no puede superar los 2000 caracteres.")]
    public string? Description { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "UserId es obligatorio y debe ser mayor que 0.")]
    public int UserId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "CreatedBy debe ser mayor que 0.")]
    public int CreatedBy { get; set; }
}
