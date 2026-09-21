using System.ComponentModel.DataAnnotations;
using Models.Tasks;

namespace Services.Tasks.ChangeStatus;

public class ChangeTaskStatusRequest
{
    [Required(ErrorMessage = "El estado es obligatorio.")]
    [EnumDataType(typeof(TaskItemStatus), ErrorMessage = "El estado debe ser 'pending', 'inProgress' o 'done'.")]
    public TaskItemStatus Status { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "UpdatedBy debe ser mayor que 0.")]
    public int UpdatedBy { get; set; }
}
