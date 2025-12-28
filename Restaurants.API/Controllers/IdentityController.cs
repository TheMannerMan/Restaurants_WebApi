using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restaurants.Application.Users.Commands.AssignUserRole;
using Restaurants.Application.Users.Commands.RemoveUserRole;
using Restaurants.Application.Users.Commands.UpdateUserDetails;
using Restaurants.Domain.Constants;

namespace Restaurants.API.Controllers;

[ApiController]
[Route("api/identity")]
public class IdentityController(IMediator mediator) : ControllerBase
{
	[HttpPatch("user")]
	[Authorize]
	public async Task<IActionResult> UpdateUserDetail(UpdateUserDetailsCommand command)
	{
		await mediator.Send(command);
		return NoContent();
	}

	[HttpPost("userRole")]
	[Authorize(Roles = UserRoles.Admin)]
	public async Task<IActionResult> AssignUserRole(AssignUserRoleCommand command)
	{
		await mediator.Send(command);
		return NoContent();
	}

	[HttpDelete("userserRole")]
	[Authorize(Roles = UserRoles.Admin)]
	public async Task<IActionResult> UnAssignUserRole(UnAssignedUserCommand command)
	{
		await mediator.Send(command);
		return NoContent();
	}
}
