using System.ComponentModel.DataAnnotations;

namespace Services.Tasks.UpdateAdditionalInfo;

public class UpdateTaskAdditionalInfoRequest
{
    [Required(ErrorMessage = "La prioridad es obligatoria.")]
    [RegularExpression("^(low|medium|high)$", ErrorMessage = "La prioridad debe ser low, medium o high.")]
    public string Priority { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "UpdatedBy debe ser mayor que 0.")]
    public int UpdatedBy { get; set; }
}
