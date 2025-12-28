using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Restaurants.Domain.Entities;
using System.Security.Claims;

namespace Restaurants.Infrastructure.Authorization;

/// <summary>
/// Custom Claims Factory that extends the default Identity behavior.
/// 
/// PURPOSE:
/// This factory is invoked automatically by ASP.NET Core Identity when a user logs in.
/// It creates the user's identity (ClaimsPrincipal) which contains all the Claims (user information)
/// that will be stored in the authentication token (cookie or JWT).
/// 
/// WHY WE NEED THIS:
/// By default, Identity only adds standard claims (ID, email, roles).
/// Our User entity has custom properties (Nationality, DateOfBirth) that we want to include
/// as claims so they're available throughout the application without database queries.
/// 
/// WHEN IS THIS CALLED:
/// - When a user logs in (cookie/JWT is created)
/// - When authentication token is refreshed
/// - When user identity needs to be reconstructed from the token
/// </summary>
public class RestaurantsUserClaimsPrincipalFactory(
		UserManager<User> userManager,           // Manages user operations (find user, validate, etc.)
		RoleManager<IdentityRole> roleManager,   // Manages role operations
		IOptions<IdentityOptions> options)       // Identity configuration settings
	: UserClaimsPrincipalFactory<User, IdentityRole>(userManager, roleManager, options)
{
	/// <summary>
	/// Overrides the default CreateAsync method to add custom claims to the user's identity.
	/// 
	/// FLOW:
	/// 1. Create base identity with standard claims (ID, username, email, roles)
	/// 2. Add our custom claims (Nationality, DateOfBirth)
	/// 3. Package everything into a ClaimsPrincipal
	/// 4. This ClaimsPrincipal is serialized into the authentication token
	/// </summary>
	/// <param name="user">The user who is logging in</param>
	/// <returns>ClaimsPrincipal containing all user claims (standard + custom)</returns>
	public override async Task<ClaimsPrincipal> CreateAsync(User user)
	{
		// STEP 1: Generate the default claims identity
		// This calls the base class method which creates standard claims like:
		// - NameIdentifier (User ID)
		// - Name (Username)
		// - Email
		// - Role claims (Admin, User, etc.)
		var id = await GenerateClaimsAsync(user);

		// STEP 2: Add custom claims from our User entity
		// These will be included in the authentication token (cookie/JWT)
		// and accessible via User.FindFirst("ClaimName") in controllers/services

		// Add Nationality claim if the user has provided their nationality
		// Later accessible via: User.FindFirst("Nationality")?.Value
		if(user.Nationality != null)
		{
			id.AddClaim(new Claim("Nationality", user.Nationality));
		}

		// Add DateOfBirth claim formatted as ISO 8601 date (yyyy-MM-dd)
		// Later accessible via: User.FindFirst("DateOfBirth")?.Value
		// Can be used for age verification policies, personalization, etc.
		if(user.DateOfBirth != null)
		{
			id.AddClaim(new Claim("DateOfBirth", user.DateOfBirth.Value.ToString("yyyy-MM-dd")));
		}

		// STEP 3: Create and return the ClaimsPrincipal
		// This is what ASP.NET Core uses to represent the authenticated user
		// It will be serialized into the authentication token (cookie or JWT)
		// and deserialized on every request to populate HttpContext.User
		return new ClaimsPrincipal(id);
	}
}

// ============================================================================
// HOW TO USE THESE CLAIMS IN YOUR APPLICATION:
// ============================================================================
//
// In Controllers/Services, you can access these claims via the User property:
// 
//   var nationality = User.FindFirst("Nationality")?.Value;
//   var dateOfBirth = User.FindFirst("DateOfBirth")?.Value;
// 
// In Authorization Policies:
// 
//   services.AddAuthorization(options =>
//   {
//       options.AddPolicy("SwedishOnly", policy =>
//           policy.RequireClaim("Nationality", "Sweden"));
//            
//       options.AddPolicy("Adult", policy =>
//           policy.RequireAssertion(context =>
//           {
//               var dob = context.User.FindFirst("DateOfBirth")?.Value;
//               return DateTime.Parse(dob).AddYears(18) <= DateTime.Now;
//           }));
//   });
// 
// IMPORTANT NOTES:
// - Claims are stored in the token, so updating user properties won't reflect
//   until the user logs in again (token is regenerated)
// - Don't store sensitive data in claims (they can be decoded from JWT tokens)
// - Keep claims small to avoid large cookie/token sizes
// ============================================================================
