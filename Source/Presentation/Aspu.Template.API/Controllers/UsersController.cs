using Aspu.L2.Application.Application.Users.Commands;
using Aspu.L2.Application.Application.Users.Queries;
using Aspu.Template.API.Infrastructure;
using Aspu.Template.Domain.Common;
using Aspu.Template.Domain.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Aspu.L2.API.Controllers;

public class UsersController : ApiController
{
    [AllowAnonymous]
    [HttpPost(ControllerMethods.Create)]
    public async Task<Result> AddUser(CreateUserCommand command) => await Mediator.Send(command);

    [HttpGet(ControllerMethods.List)]
    public async Task<Result<List<ApplicationUserDto>>> GetAllUsers() => await Mediator.Send(new GetAllUsersQuery());

    [HttpGet(ControllerMethods.Load)]
    public async Task<Result<ApplicationUserDto?>> GetUserById(string id) => await Mediator.Send(new GetUserByIdQuery(id));

    [HttpGet("login")]
    public async Task<Result<ApplicationUserDto?>> GetUserByLogin(string login) => await Mediator.Send(new GetUserByLoginQuery(login));

    [Authorize("AdminPolicy")]
    [HttpPost(ControllerMethods.Update)]
    public async Task<Result> UpdateUser(UpdateUserCommand command) => await Mediator.Send(command);

    [Authorize("AdminPolicy")]
    [HttpPost("update/password")]
    public async Task<Result> UpdatePassword(UpdatePasswordCommand command) => await Mediator.Send(command);

    [Authorize("AdminPolicy")]
    [HttpPost(ControllerMethods.Delete)]
    public async Task<Result> DeleteUser(DeleteUserCommand command) => await Mediator.Send(command);

}
