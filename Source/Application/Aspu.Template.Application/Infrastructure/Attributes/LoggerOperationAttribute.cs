namespace Aspu.Template.Application.Infrastructure.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class LoggerOperationAttribute(string operationName) : Attribute
{
    public string OperationName { get; } = operationName;
}
