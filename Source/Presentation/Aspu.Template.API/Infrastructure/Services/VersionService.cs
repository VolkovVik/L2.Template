using Aspu.Template.Application.Interfaces;
using System.Reflection;

namespace Aspu.Template.API.Infrastructure.Services;

public class VersionService : IVersionService
{
    private readonly bool _isDebug;

    public VersionService(IHostEnvironment hostEnvironment) =>
        _isDebug = hostEnvironment.IsDevelopment();

    public string ApplicationVersion => GetApplicationVersion();

    private string GetApplicationVersion()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var version = assembly.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version ?? "0.0.0";
        /// var version = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "0.0.0";
        return !_isDebug ? version : $"{version} debug";
    }
}
