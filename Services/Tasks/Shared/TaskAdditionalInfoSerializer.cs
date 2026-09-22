using System.Text.Json;
using System.Text.Json.Serialization;
using Common.Exceptions;

namespace Services.Tasks.Shared;

public static class TaskAdditionalInfoSerializer
{
    private static readonly HashSet<string> ValidPriorities = new(StringComparer.OrdinalIgnoreCase)
    {
        "low", "medium", "high"
    };

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = false
    };

    public static string? Serialize(TaskAdditionalInfoDto? info)
    {
        if (info is null)
        {
            return null;
        }

        var hasContent =
            !string.IsNullOrWhiteSpace(info.Priority)
            || info.DueDate.HasValue
            || (info.Tags is { Count: > 0 })
            || (info.Metadata is { Count: > 0 });

        if (!hasContent)
        {
            return null;
        }

        NormalizeAndValidate(info);

        return JsonSerializer.Serialize(info, JsonOptions);
    }

    public static TaskAdditionalInfoDto? Deserialize(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return null;
        }

        try
        {
            return JsonSerializer.Deserialize<TaskAdditionalInfoDto>(json, JsonOptions);
        }
        catch (JsonException)
        {
            return null;
        }
    }

    public static void NormalizeAndValidate(TaskAdditionalInfoDto info)
    {
        if (!string.IsNullOrWhiteSpace(info.Priority))
        {
            var priority = info.Priority.Trim().ToLowerInvariant();

            if (!ValidPriorities.Contains(priority))
            {
                throw new BadRequestException("La prioridad debe ser low, medium o high.");
            }

            info.Priority = priority;
        }
        else
        {
            info.Priority = null;
        }

        if (info.Tags is { Count: > 0 })
        {
            info.Tags = info.Tags
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .Select(t => t.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (info.Tags.Count == 0)
            {
                info.Tags = null;
            }
        }
    }

    public static string NormalizePriorityFilter(string priority)
    {
        var normalized = priority.Trim().ToLowerInvariant();

        if (!ValidPriorities.Contains(normalized))
        {
            throw new BadRequestException("La prioridad debe ser low, medium o high.");
        }

        return normalized;
    }
}
