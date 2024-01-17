using Aspu.Template.Domain.Dto;
using AutoMapper;
using System.Reflection;

namespace Aspu.Template.Application.Infrastructure.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        //var references = Assembly.GetEntryAssembly()?.GetReferencedAssemblies().ToList();
        //var assemblies = references?.Select(AssemblyLoadContext.Default.LoadFromAssemblyName).ToList();

        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        var notDynamicAssemblies = assemblies?.Where(a => !a.IsDynamic).ToArray() ?? [];
        foreach (var assembly in notDynamicAssemblies)
        {
            ApplyMappingsFromAssembly(assembly);
        }
    }

    private void ApplyMappingsFromAssembly(Assembly assembly)
    {
        var types = assembly.GetExportedTypes()
            .Where(t => Array.Exists(t.GetInterfaces(), i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMapFrom<>)))
            .ToList();
        foreach (var type in types)
        {
            var instance = Activator.CreateInstance(type);
            var methodInfo = type.GetMethod("Mapping") ?? type.GetInterface("IMapFrom`1")?.GetMethod("Mapping");
            methodInfo?.Invoke(instance, new object[] { this });
        }
    }
}
