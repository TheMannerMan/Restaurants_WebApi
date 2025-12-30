using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Restaurants.Application.Users;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mime;
using System.Text;

namespace Restaurants.Infrastructure.Authorization.Requirements;

internal class MinimumAgeRequirementsHandler(ILogger<MinimumAgeRequirementsHandler> logger,
    IUserContext userContext)
    : AuthorizationHandler<MinimumAgeRequirements>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
        MinimumAgeRequirements requirement)
    {
        var CurrentUser = userContext.GetCurrentUser();

        logger.LogInformation("User: {Email}, date of birth {DoB} - Handling MinimumAgeRequirments",
            CurrentUser.Email, CurrentUser.DateOfBirth);

        if(CurrentUser.DateOfBirth == null)
        {
            logger.LogWarning("User date of birth is null");
            context.Fail();
            return Task.CompletedTask;
        }
        
        if (CurrentUser.DateOfBirth.Value.AddYears(requirement.MinimumAge) <= DateOnly.FromDateTime(DateTime.Today))
        {
            logger.LogInformation("Authorization succeeded");
            context.Succeed(requirement);
        }

        else
        {
            context.Fail();
        }

            return Task.CompletedTask;
    }
}
