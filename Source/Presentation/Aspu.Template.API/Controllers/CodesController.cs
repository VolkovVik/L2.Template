using Aspu.Template.Application.Application;
using Aspu.Template.Application.Application.Test.Queries;
using Aspu.Template.Domain.Common;
using Aspu.Template.Domain.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Aspu.L2.API.Controllers;

public class CodesController : ApiController
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<Result<ApplicationUserDto>> Load() => await Mediator.Send(new GetCodeByIdQuery());

    [HttpGet("load")]
    public async Task<Result<ApplicationUserDto>> Load1() => await Mediator.Send(new GetCodeByIdQuery());

    [HttpGet("test")]
    [AllowAnonymous]
    public async Task<Result<GetQueryResponse?>> Test() => await Mediator.Send(new GetQuery());
}
