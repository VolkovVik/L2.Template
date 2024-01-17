using Aspu.Template.Application.Infrastructure.Attributes;
using Aspu.Template.Domain.Common;
using FluentValidation;
using MediatR;
using Serilog;
using System.Reflection;

namespace Aspu.Template.Application.Infrastructure.Behaviours;

public class LoggerBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : Result
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        TResponse response;
        var operation = LoggerBehaviour<TRequest, TResponse>.GetOperationName(request);

        try
        {
            response = await next();
            LoggerBehaviour<TRequest, TResponse>.LogResponse(operation, response);
        }
        catch (Exception e)
        {
            LoggerBehaviour<TRequest, TResponse>.LogException(operation, e);
            throw;
        }
        return response;
    }

    private static string GetOperationName(TRequest request)
    {
        var type = typeof(TRequest);
        var attribute = type.GetCustomAttribute<LoggerOperationAttribute>();
        if (attribute == null) return string.Empty;

        var requestObj = GetRequestData(request);
        requestObj += string.IsNullOrWhiteSpace(requestObj) ? string.Empty : " ";
        var value = $"{attribute.OperationName} {requestObj}: ";
        return value;
    }

    private static void LogResponse(string operationName, TResponse response)
    {
        if (string.IsNullOrWhiteSpace(operationName)) return;

        string? message;
        if (response.IsSuccess)
        {
            message = operationName + "OK." + GetResponseData(response);
            Log.Information(message);
            return;
        }
        message = operationName + $"ERROR." + (response.Messages?.Count > 0 ? $" {string.Join(", ", response.Messages)}" : string.Empty);
        Log.Error(message);
    }

    private static void LogException(string operationName, Exception ex)
    {
        if (string.IsNullOrWhiteSpace(operationName)) return;

        Log.Error(operationName, ex);
    }

    private static string GetRequestData(object? data)
    {
        if (data == null) return string.Empty;

        var type = data.GetType();
        if (type.IsValueType) return $"[{{Value={data}}}]";

        var properties = GetProperties(type)
            .Union(GetProperties(type.BaseType))
            .Union(GetProperties(type.BaseType?.BaseType))
            .Select(x => GetValue(x, data))
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToArray();
        var value = properties.Length == 0 ? string.Empty : $" [{{{string.Join(", ", properties)}}}]";
        return value;
    }

    private static string GetValue(PropertyInfo x, object data)
    {
        var attribute = x.GetCustomAttribute<LoggerPropertyAttribute>();
        if (attribute == null) return string.Empty;

        var attributeValue = x.GetValue(data);
        if (attributeValue == null) return string.Empty;

        if (attributeValue is not Array items)
            return $"{attribute.DisplayName}={attributeValue}";

        var list = items.OfType<string>();
        var str = $"{attribute.DisplayName}=[{string.Join(", ", list)}]";
        return str;
    }

    private static PropertyInfo[] GetProperties(Type? type)
    {
        var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly;
        var properties = type?.GetProperties(flags) ?? [];
        return properties;
    }

    private static string GetResponseData(TResponse response)
    {
        var type = typeof(TResponse);
        if (!type.IsGenericType || type.GetGenericTypeDefinition() != typeof(Result<>)) return string.Empty;

        var property = type.GetProperty("Value");
        if (property == null) return string.Empty;

        var value = property.GetValue(response);
        var result = GetRequestData(value);
        return result;
    }
}

