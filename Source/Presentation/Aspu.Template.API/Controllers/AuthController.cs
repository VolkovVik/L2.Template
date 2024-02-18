using Aspu.L2.Application.Application.Auth;
using Aspu.Template.Application.Application.Auth.Queries;
using Aspu.Template.Application.Application.Test.Queries;
using Aspu.Template.Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Aspu.L2.API.Controllers;

public class AuthController : ApiController
{
    [AllowAnonymous]
    [HttpGet("/api/ping")]
    public Result Ping() => Result.Ok();

    [AllowAnonymous]
    [HttpPost("/api/login")]
    public async Task<Result<GetTokenQueryResponse?>> GetToken(GetTokenQuery query) => await Mediator.Send(query);

    [AllowAnonymous]
    [HttpPost("/api/checkToken")]
    public async Task<Result<CheckTokenQueryResponse>> CheckToken(CheckTokenQuery query) => await Mediator.Send(query);

    [HttpGet("/api/refreshToken")]
    public async Task<Result<RefreshTokenQueryResponse?>> RefrashToken() => await Mediator.Send(new RefreshTokenQuery());
}
