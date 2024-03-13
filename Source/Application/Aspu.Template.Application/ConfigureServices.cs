﻿using Aspu.Template.Application.Implementation;
using Aspu.Template.Application.Infrastructure.Behaviours;
using Aspu.Template.Application.Quartz;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Aspu.Template.Application;

public static class ConfigureServices
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();

        services.Scan(scan =>
            scan.FromCallingAssembly()
                .AddClasses(classes => classes.AssignableTo<IScrutorScopedService>())
                .AsImplementedInterfaces()
                .WithScopedLifetime());

        services.AddApplicationQuartzServices(configuration);

        services.AddAutoMapper(assemblies);
        services.AddValidatorsFromAssembly(assembly);
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(assembly);
            config.AddBehavior(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
            //configuration.AddBehavior(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehaviour<,>));
            config.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
            config.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggerBehaviour<,>));
            //configuration.AddBehavior(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));
        });
        return services;
    }
}
