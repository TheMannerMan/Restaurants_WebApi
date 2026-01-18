using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Moq;
using Restaurants.Application.Users;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Repositories;
using Restaurants.Infrastructure.Authorization.Requirements;
using System.Threading.Tasks;
using Xunit;

namespace Restaurants.Infrastructure.Tests.Authorization.Requirements;

public class CreatedMultipleRestaurantsRequirementHandlerTests
{
    private readonly Mock<IRestaurantsRepository> _restaurantsRepositoryMock;
    private readonly Mock<IUserContext> _userContextMock;
    private readonly CreatedMultipleRestaurantsRequirementHandler _handler;

    public CreatedMultipleRestaurantsRequirementHandlerTests()
    {
        _restaurantsRepositoryMock = new Mock<IRestaurantsRepository>();
        _userContextMock = new Mock<IUserContext>();
        _handler = new CreatedMultipleRestaurantsRequirementHandler(_restaurantsRepositoryMock.Object, _userContextMock.Object);
    }

    [Fact]
    public async Task HandleRequirementAsync_UserHasCreatedMultipleRestaurants_ShouldSucceed()
    {
        // Arrange
        var currentUser = new CurrentUser("1", "test@test.com", [], null, null);

        _userContextMock.Setup(m => m.GetCurrentUser()).Returns(currentUser);

        var restaurants = new List<Restaurant>()
        {
            new()
            {
                OwnerId = currentUser.Id
            },
            new()
            {
                OwnerId = currentUser.Id
            },
            new()
            {
                OwnerId = "2"
            },
        };

        _restaurantsRepositoryMock.Setup(m => m.GetAllAsync()).ReturnsAsync(restaurants);

        var requirment = new CreatedMultipleRestaurantsRequirement(2);

        var context = new AuthorizationHandlerContext([requirment], null, null);

        // act 

        await _handler.HandleAsync(context);

        //assert
        context.HasSucceeded.Should().BeTrue();
    }

    [Fact]
    public async Task HandleRequirementAsync_UserHasNotCreatedMultipleRestaurants_ShouldFail()
    {
        // Arrange
        var currentUser = new CurrentUser("1", "test@test.com", [], null, null);

        _userContextMock.Setup(m => m.GetCurrentUser()).Returns(currentUser);

        var restaurants = new List<Restaurant>()
        {
            new()
            {
                OwnerId = currentUser.Id
            },
            new()
            {
                OwnerId = "2"
            },
        };

        _restaurantsRepositoryMock.Setup(m => m.GetAllAsync()).ReturnsAsync(restaurants);

        var requirement = new CreatedMultipleRestaurantsRequirement(2);

        var context = new AuthorizationHandlerContext([requirement], null, null);

        // act 

        await _handler.HandleAsync(context);

        //assert
        context.HasSucceeded.Should().BeFalse();
        context.HasFailed.Should().BeTrue();
    }
}
