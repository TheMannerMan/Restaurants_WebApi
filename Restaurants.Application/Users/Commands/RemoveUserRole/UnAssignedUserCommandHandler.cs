using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Restaurants.Domain.Constants;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Restaurants.Application.Users.Commands.RemoveUserRole;

public class UnAssignedUserCommandHandler(ILogger<UnAssignedUserCommandHandler> logger,
	RoleManager<IdentityRole> roleManager, UserManager<User> userManager) : IRequestHandler<UnAssignedUserCommand>
{
	public async Task Handle(UnAssignedUserCommand request, CancellationToken cancellationToken)
	{
		logger.LogInformation("Unassign user role {@request}", request);

		var user = await userManager.FindByEmailAsync(request.UserEmail)
			?? throw new NotFoundException(nameof(User), request.UserEmail);

		var role = await roleManager.FindByNameAsync(request.RoleName)
			?? throw new NotFoundException(nameof(IdentityRole), request.RoleName);

		await userManager.RemoveFromRoleAsync(user, request.RoleName!);
	}
}
