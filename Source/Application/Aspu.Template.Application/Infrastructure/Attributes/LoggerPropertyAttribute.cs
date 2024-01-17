namespace Aspu.Template.Application.Infrastructure.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class LoggerPropertyAttribute(string displayName) : Attribute
    {
        public string DisplayName { get; } = displayName;
    }
}
