using MediatR;
using Serilog;

namespace Aspu.Template.Application.Infrastructure.Behaviours;

public class UnhandledExceptionBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (Exception ex)
        {
            var requestName = typeof(TRequest).Name;
            Log.Error(ex, "Unhandled Exception for Request {Name} {@Request}", requestName, request);
            throw;
        }
    }
}