using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Restaurants.Application.Restaurants.Commands.UpdateRestaurant;
using Restaurants.Application.Users;
using Restaurants.Domain.Constants;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Exceptions;
using Restaurants.Domain.Interfaces;
using Restaurants.Domain.Repositories;
using Xunit;

namespace Restaurants.Application.Tests.Restaurants.Commands.UpdateRestaurant;

// DONE Step 1: Create a new test class for UpdateRestaurantCommandHandler DONE


// Done Step 2: Define a test scenario to cover the "standard scenario" - user is properly authorized, mapper.Map method is invoked and then the SaveChanges() as well. DONE
// Step 3: Define a test scenario to capture the NotFoundException scenario
// Step 4: Define a test scenario to capture the ForbidException scenario

public class UpdateRestaurantCommandHandlerTests
{
    private readonly Mock<ILogger<UpdateRestaurantCommandHandler>> _loggerMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IRestaurantsRepository> _restaurantRepositoryMock;
    private readonly Mock<IRestaurantAuthorizationService> _restaurantAuthorizationServiceMock;

    private readonly UpdateRestaurantCommandHandler _handler;

    public UpdateRestaurantCommandHandlerTests()
    {
        _loggerMock = new Mock<ILogger<UpdateRestaurantCommandHandler>>();
        _mapperMock = new Mock<IMapper>();
        _restaurantRepositoryMock = new Mock<IRestaurantsRepository>();
        _restaurantAuthorizationServiceMock = new Mock<IRestaurantAuthorizationService>();

        _handler = new UpdateRestaurantCommandHandler(
            _loggerMock.Object,
            _mapperMock.Object,
            _restaurantRepositoryMock.Object,
            _restaurantAuthorizationServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ForValidRequest_ShouldUpdateRestaurants()
    {
        // Arrange

        var restaurantId = 1;

        var command = new UpdateRestaurantCommand
        {
            Id = restaurantId
        };
        
        var restaurant = new Restaurant
        {
            Id = restaurantId
        };

        _restaurantRepositoryMock.Setup(r => r.GetByIdAsync(restaurantId))
            .ReturnsAsync(restaurant);

        _restaurantAuthorizationServiceMock.Setup(a => a.Authorize(restaurant, 
            ResourceOperation.Update)).Returns(true);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert

        _restaurantRepositoryMock.Verify(r => r.SaveChanges(), Times.Once);   
        _mapperMock.Verify(m => m.Map(command, restaurant), Times.Once);
    }

    [Fact]
    public async Task Handle_WithUnauthorizedUser_ShouldThrowForbidException()
    {
        // Arrange
        var restaurantId = 1;
        var command = new UpdateRestaurantCommand() { Id = restaurantId };
        var restaurant = new Restaurant() { Id = restaurantId };

        _restaurantRepositoryMock.Setup(r => r.GetByIdAsync(restaurantId)).ReturnsAsync((restaurant));
        _restaurantAuthorizationServiceMock.Setup(a => a.Authorize(restaurant, 
            ResourceOperation.Update)).Returns(false);


        // Act and Assert
        await _handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should()
            .ThrowAsync<ForbidException>();

        // Verify that SaveChanges should never be called when restaurant is not found
        _restaurantRepositoryMock.Verify(r => r.SaveChanges(), Times.Never);
    }

    [Fact]
    public async Task Handle_ForNonExistingRestaurant_ShouldThrowNotFoundException()
    {
        // Arrange
        var restarantId = 999;
        var command = new UpdateRestaurantCommand();

        _restaurantRepositoryMock.Setup(r => r.GetByIdAsync(restarantId)).ReturnsAsync((Restaurant?)null);

        ////act
        //Func<Task> act = async () => { await _handler.Handle(command, CancellationToken.None); };

        ////Assert
        //await act.Should().ThrowAsync<NotFoundException>();

       // Act and Assert
       await _handler.Invoking(h => h.Handle(command, CancellationToken.None))
           .Should()
           .ThrowAsync<NotFoundException>();

        // Verify that SaveChanges should never be called when restaurant is not found
        _restaurantRepositoryMock.Verify(r => r.SaveChanges(), Times.Never);
    }
}
