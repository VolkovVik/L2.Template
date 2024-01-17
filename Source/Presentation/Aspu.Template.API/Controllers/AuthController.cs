using Aspu.L2.Application.Application.Auth;
using Aspu.Template.Application.Application.Auth.Queries;
using Aspu.Template.Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Aspu.L2.API.Controllers;

public class AuthController : ApiController
{
    [AllowAnonymous]
    [HttpGet("ping")]
    public Result Ping() => Result.Ok();

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<Result<GetTokenQueryResponce>> GetToken(GetTokenQuery query) => await Mediator.Send(query);

    [AllowAnonymous]
    [HttpPost("/api/checkToken")]
    public async Task<Result<CheckTokenQueryResponce>> CheckToken(CheckTokenQuery query) => await Mediator.Send(query);

    [HttpGet("/api/refreshToken")]
    public async Task<Result<RefreshTokenQueryResponce>> RefrashToken() => await Mediator.Send(new RefreshTokenQuery());
}
