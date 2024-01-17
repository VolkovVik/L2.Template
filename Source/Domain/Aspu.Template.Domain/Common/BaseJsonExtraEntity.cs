using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Aspu.Template.Domain.Common;

public abstract class BaseJsonExtraEntity : BaseAuditableEntity, IJsonExtraEntity
{
    public string? ExtraFieldsJson { get; set; }

    [NotMapped]
    [JsonIgnore]
    public IDictionary<string, object> Fields
    {
        get => string.IsNullOrWhiteSpace(ExtraFieldsJson) ? [] : JsonSerializer.Deserialize<Dictionary<string, object>>(ExtraFieldsJson) ?? [];
        set => ExtraFieldsJson = JsonSerializer.Serialize(value) ?? "{}";
    }

    protected void SetFieldsValue(string key, object value)
    {
        if (string.IsNullOrWhiteSpace(key) || value == null) return;

        var fields = Fields ?? new Dictionary<string, object>();
        var isSuccess = fields.TryAdd(key, value);
        if (!isSuccess)
            fields["key"] = value;

        ExtraFieldsJson = JsonSerializer.Serialize(fields) ?? "{}";
    }
}
