using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Restaurants.Application.Users;

// IHttpContextAccessor provides access to the current HttpContext (if available) through the request pipeline.
// Its implementation (HttpContextAccessor) uses AsyncLocal<T> to store and retrieve the current HTTP context,
// allowing access to request-specific information (like user claims, headers, route data) from services outside of controllers.
public class UserContext(IHttpContextAccessor httpContextAccessor) : IUserContext
{
	public CurrentUser? GetCurrentUser()
	{
		var user = httpContextAccessor.HttpContext?.User;
		if (user is null)
		{
			throw new InvalidOperationException("User context is not available");
		}

		if (user.Identity == null || !user.Identity.IsAuthenticated)
		{
			return null;
		}


		var userId = user.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)!.Value;
		var userEmail = user.FindFirst(c => c.Type == ClaimTypes.Email)!.Value;
		var userRoles = user.FindAll(ClaimTypes.Role)!.Select(c => c.Value);
		var nationality = user.FindFirst(c => c.Type == "Nationality")?.Value;
        var dateOfBirthString = user.FindFirst(c => c.Type == "DateOfBirth")?.Value;
		var dateOfBirth = dateOfBirthString == null
			? (DateOnly?)null : DateOnly.ParseExact(dateOfBirthString, "yyyy-MM-dd");

        return new CurrentUser(userId, userEmail, userRoles, nationality, dateOfBirth);

	}
}
