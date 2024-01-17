namespace Aspu.Template.Domain.Common;

public interface IJsonExtraEntity
{
    string? ExtraFieldsJson { get; set; }
    IDictionary<string, object> Fields { get; set; }
}
