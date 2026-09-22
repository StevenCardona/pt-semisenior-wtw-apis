using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace Services.Tasks.Shared;

public class TaskAdditionalInfoDto
{
    [RegularExpression("^(low|medium|high)$", ErrorMessage = "La prioridad debe ser low, medium o high.")]
    public string? Priority { get; set; }

    public DateOnly? DueDate { get; set; }

    public List<string>? Tags { get; set; }

    public Dictionary<string, JsonElement>? Metadata { get; set; }
}
