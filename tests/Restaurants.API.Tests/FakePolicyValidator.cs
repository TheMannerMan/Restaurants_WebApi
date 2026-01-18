using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Restaurants.API.Tests;

/// <summary>
/// Fake implementation of IPolicyEvaluator used in integration tests.
/// This class bypasses the real authorization logic and always returns a successful authentication result
/// with a fake authenticated user. This allows tests to focus on controller and business logic
/// without requiring valid JWT tokens or complex authorization setup.
/// </summary>
internal class FakePolicyValidator : IPolicyEvaluator
{
    /// <summary>
    /// Authenticates the request by creating a fake ClaimsPrincipal with predetermined claims.
    /// Instead of validating a real JWT token, this always returns success with a mock user.
    /// </summary>
    /// <param name="policy">The authorization policy (ignored in tests)</param>
    /// <param name="context">The HTTP context (not used for authentication in this fake implementation)</param>
    /// <returns>Always returns an AuthenticateResult.Success containing a fake authenticated user</returns>
    public Task<AuthenticateResult> AuthenticateAsync(AuthorizationPolicy policy, HttpContext context)
    {
        // Create a fake user with predefined claims that represent an authenticated Admin user.
        var claimsPrincipal = new ClaimsPrincipal();

        claimsPrincipal.AddIdentity(new ClaimsIdentity(new[]
        {
            // NameIdentifier claim simulates the user's ID in the system
            new Claim(ClaimTypes.NameIdentifier, "1"),
            // Role claim simulates that this user has Admin privileges
            new Claim(ClaimTypes.Role, "Admin")
        }));

        // Wrap the fake principal in an AuthenticationTicket and return success.
        // Tests can use this authenticated user without needing real credentials.
        var ticket = new AuthenticationTicket(claimsPrincipal, "Test");
        var result = AuthenticateResult.Success(ticket);
        return Task.FromResult(result);
    }

    /// <summary>
    /// Authorizes the request by always returning success.
    /// This bypasses all policy-based authorization checks (e.g., role checks, permission requirements).
    /// </summary>
    /// <param name="policy">The authorization policy being evaluated (ignored in tests)</param>
    /// <param name="authenticationResult">The result from AuthenticateAsync (ignored in tests)</param>
    /// <param name="context">The HTTP context (ignored in tests)</param>
    /// <param name="resource">The resource being accessed (ignored in tests)</param>
    /// <returns>Always returns PolicyAuthorizationResult.Success, allowing all requests through</returns>
    public Task<PolicyAuthorizationResult> AuthorizeAsync(AuthorizationPolicy policy, AuthenticateResult authenticationResult, HttpContext context, object? resource)
    {
        // Always authorize any request. This means [Authorize] attributes on controller actions
        // will not block the test requests, allowing the actual endpoint logic to be tested.
        var result = PolicyAuthorizationResult.Success();
        return Task.FromResult(result);
    }
}
