using Aspu.Template.Application.Implementation;
using Aspu.Template.Application.Infrastructure.Attributes;
using Aspu.Template.Domain.Common;
using FluentValidation;
using MediatR;

namespace Aspu.Template.Application.Application.Test.Queries;

[LoggerOperation("Get token")]
public record GetQuery : IRequest<Result<GetQueryResponse?>> { }

public class GetQueryValidator : AbstractValidator<GetQuery>
{
    public GetQueryValidator() { }
}

public record GetQueryResponse
{
    public List<string> Items { get; set; } = [];
    public string Value => string.Join(',', Items?.Where(x => !string.IsNullOrWhiteSpace(x)) ?? []);
}

public class GetQueryHandler(IEnumerable<IService> services) : IRequestHandler<GetQuery, Result<GetQueryResponse?>>
{
    private readonly IService[] _services = services.ToArray();

    public async Task<Result<GetQueryResponse?>> Handle(GetQuery request, CancellationToken cancellationToken)
    {
        var items = _services.Select(x => x.GetValue());
        var response = new GetQueryResponse { Items = items.ToList() };
        return Result<GetQueryResponse?>.Ok(response);
    }
}