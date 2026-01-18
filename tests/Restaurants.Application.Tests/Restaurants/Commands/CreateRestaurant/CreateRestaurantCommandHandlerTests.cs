using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Restaurants.Application.Restaurants.Commands.CreateRestaurant;
using Restaurants.Application.Users;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Repositories;
using Xunit;

namespace Restaurants.Application.Tests.Restaurants.Commands.CreateRestaurant;

public class CreateRestaurantCommandHandlerTests
{
    // Note: You can set up mocks in two ways:
    // 1. In the constructor (see commented code below) - good for shared setup
    // 2. Inside each test method (see below) - gives more control per test

    // Example of constructor-based setup (commented out, but shows the pattern):
    //private readonly Mock<ILogger<CreateRestaurantCommandHandler>> _loggerMock;
    //private readonly Mock<IMapper> _mapperMock;
    //private readonly Mock<IRestaurantsRepository> _restaurantsRepositoryMock;
    //private readonly Mock<IUserContext> _userContextMock;
    //private readonly CreateRestaurantCommandHandler _handler;

    //public CreateRestaurantCommandHandlerTests()
    //{
    //	_loggerMock = new Mock<ILogger<CreateRestaurantCommandHandler>>();
    //	_mapperMock = new Mock<IMapper>();
    //	_restaurantsRepositoryMock = new Mock<IRestaurantsRepository>>();
    //	_userContextMock = new Mock<IUserContext>>();

    //	_handler = new CreateRestaurantCommandHandler(
    //		_loggerMock.Object,
    //		_mapperMock.Object,
    //		_restaurantsRepositoryMock.Object,
    //		_userContextMock.Object);
    //}

    [Fact]
    public async Task Handle_ForValidCommand_ReturnsCreatedRestaurantId()
    {
        // === ARRANGE ===
        // This section sets up everything needed for the test
        
        // Step 1: Create test data (command and expected result)
        var command = new CreateRestaurantCommand();
        var restaurant = new Restaurant();

        // Step 2: Create all mock objects for the handler's dependencies
        var loggerMock = new Mock<ILogger<CreateRestaurantCommandHandler>>();
        var mapperMock = new Mock<IMapper>();
        var restaurantsRepositoryMock = new Mock<IRestaurantsRepository>();
        var userContextMock = new Mock<IUserContext>();

        // Step 3: Configure each mock - define what they should return/do
        
        // When mapper is called with the command, return the restaurant object
        mapperMock.Setup(m => m.Map<Restaurant>(command)).Returns(restaurant);

        // When repository Create is called with any Restaurant, return ID 1
        restaurantsRepositoryMock.Setup(r => r.Create(It.IsAny<Restaurant>())).ReturnsAsync(1);

        // When user context is called, return a CurrentUser with "owner-id"
        var currentUser = new CurrentUser("owner-id", "test@test.com", [], null, null);
        userContextMock.Setup(u => u.GetCurrentUser()).Returns(currentUser);

        // Step 4: Create the handler with all mocked dependencies
        var commandHandler = new CreateRestaurantCommandHandler(
            loggerMock.Object,           // .Object converts the mock to the real interface
            mapperMock.Object,
            restaurantsRepositoryMock.Object,
            userContextMock.Object);
        
        // === ACT ===
        // Run the method we want to test
        var result = await commandHandler.Handle(command, CancellationToken.None);

        // === ASSERT ===
        // Check that the results are what we expect
        
        // The handler should return 1 (the restaurant ID)
        result.Should().Be(1);
        
        // The handler should set the restaurant's owner to the current user's ID
        restaurant.OwnerId.Should().Be("owner-id");
        
        // The repository's Create method should be called exactly once
        // with the restaurant object. This checks that the data was saved.
        restaurantsRepositoryMock.Verify(r => r.Create(restaurant), Times.Once);
    }

}
