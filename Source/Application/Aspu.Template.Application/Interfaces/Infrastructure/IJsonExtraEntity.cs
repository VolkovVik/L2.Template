namespace Aspu.Template.Application.Interfaces.Infrastructure;

public interface IJsonExtraEntity
{
    string? ExtraFieldsJson { get; set; }
    IDictionary<string, object> Fields { get; set; }
}
