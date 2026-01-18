using MediatR;

namespace Restaurants.Application.Users.Commands.RemoveUserRole;

public class UnAssignedUserCommand : IRequest
{
	public string UserEmail { get; set; } = default!;
	public string RoleName { get; set; } = default!;
}

