using Aspu.Template.Application.Infrastructure.Attributes;
using Aspu.Template.Domain.Common;
using Aspu.Template.Domain.Dto;
using Aspu.Template.Domain.Entities;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Aspu.Template.Application.Application;

[LoggerOperation("Get codes by ID")]
public record GetCodeByIdQuery : IRequest<Result<ApplicationUserDto>> { }

public class GetCodeByIdQueryValidator : AbstractValidator<GetCodeByIdQuery> { }

public class GetCodeByIdQueryHandler111(IMapper mapper) : IRequestHandler<GetCodeByIdQuery, Result<ApplicationUserDto>>
{
    public async Task<Result<ApplicationUserDto>> Handle(GetCodeByIdQuery request, CancellationToken cancellationToken)
    {
        await Task.Delay(100, cancellationToken);
        var user = new ApplicationUser
        {
            Name = "1111",
            Role = "2222"
        };
        var dto = mapper.Map<ApplicationUserDto>(user);
        return dto;
    }
}