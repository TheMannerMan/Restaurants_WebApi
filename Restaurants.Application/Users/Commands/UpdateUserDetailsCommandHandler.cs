using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Exceptions;
namespace Restaurants.Application.Users.Commands
{
	/// <summary>
	/// Handles the update of user details (Nationality, DateOfBirth).
	/// 
	/// IUserStore<User>: This is ASP.NET Core Identity's built-in repository abstraction for user data access.
	/// - It provides CRUD operations: FindByIdAsync(), UpdateAsync(), CreateAsync(), DeleteAsync()
	/// - The default implementation (UserStore) uses Entity Framework Core to interact with the database
	/// - This abstraction allows you to work with Identity users without directly touching DbContext
	/// - It's part of the repository pattern built into Identity, separating data access from business logic
	/// </summary>
	public class UpdateUserDetailsCommandHandler(ILogger<UpdateUserDetailsCommandHandler> logger,
		IUserContext userContext,
		IUserStore<User> userStore) : IRequestHandler<UpdateUserDetailsCommand>
	{
		public async Task Handle(UpdateUserDetailsCommand request, CancellationToken cancellationToken)
		{
			var user = userContext.GetCurrentUser();

			logger.LogInformation("Updating user: {UserId}, with {@Request}", user!.Id, request);

			var dbUser = await userStore.FindByIdAsync(user!.Id, cancellationToken);
			if (dbUser == null)
			{
				throw new NotFoundException(nameof(User), user!.Id);
			}

			dbUser.Nationality = request.Nationality;
			dbUser.DateOfBirth = request.DateOfBirth;

			await userStore.UpdateAsync(dbUser, cancellationToken);
		}
	}
}
