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

/// <summary>
/// Test class for UpdateRestaurantCommandHandler
/// 
/// PURPOSE:
/// Tests that the handler correctly orchestrates the restaurant update process.
/// We test the FLOW (does it call the right methods in the right order?),
/// not the implementation details (that's what RestaurantsProfileTests, etc. do).
/// 
/// WHY MOCKS?
/// Handler depends on four external services. We replace them with mocks so we can:
/// - Test handler logic in isolation
/// - Control exactly what each dependency returns
/// - Verify that handler calls them correctly
/// 
/// KEY INSIGHT:
/// If handler logic changes and forgets to call SaveChanges(), this test catches it.
/// That's the real value of unit testing - catching orchestration mistakes.
/// </summary>
public class UpdateRestaurantCommandHandlerTests
{
    // ============================================================================
    // MOCKS: Fake implementations of handler's dependencies
    // ============================================================================
    
    private readonly Mock<ILogger<UpdateRestaurantCommandHandler>> _loggerMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IRestaurantsRepository> _restaurantRepositoryMock;
    private readonly Mock<IRestaurantAuthorizationService> _restaurantAuthorizationServiceMock;

    // The actual handler we're testing
    private readonly UpdateRestaurantCommandHandler _handler;

    /// <summary>
    /// SETUP - Runs before each test
    /// Creates fresh mock objects and handler instance for test isolation.
    /// Each test gets a clean slate so tests don't interfere with each other.
    /// </summary>
    public UpdateRestaurantCommandHandlerTests()
    {
        _loggerMock = new Mock<ILogger<UpdateRestaurantCommandHandler>>();
        _mapperMock = new Mock<IMapper>();
        _restaurantRepositoryMock = new Mock<IRestaurantsRepository>();
        _restaurantAuthorizationServiceMock = new Mock<IRestaurantAuthorizationService>();

        // Inject all mocks into handler
        _handler = new UpdateRestaurantCommandHandler(
            _loggerMock.Object,
            _mapperMock.Object,
            _restaurantRepositoryMock.Object,
            _restaurantAuthorizationServiceMock.Object);
    }

    // ============================================================================
    // TEST 1: Happy path - everything works correctly
    // ============================================================================
    
    /// <summary>
    /// SCENARIO: Authorized user updates an existing restaurant
    /// 
    /// EXPECTED FLOW:
    /// 1. Handler fetches restaurant from database ?
    /// 2. Handler checks authorization ?
    /// 3. Handler maps command data to restaurant object ?
    /// 4. Handler saves changes to database ?
    /// 
    /// WHAT WE TEST:
    /// - That Map() is called exactly once
    /// - That SaveChanges() is called exactly once
    /// 
    /// WHY THIS MATTERS:
    /// If future code removes the SaveChanges() call, this test fails.
    /// Without SaveChanges(), changes are never persisted to database.
    /// </summary>
    [Fact]
    public async Task Handle_ForValidRequest_ShouldUpdateRestaurants()
    {
        // Arrange: Set up test data
        var restaurantId = 1;

        var command = new UpdateRestaurantCommand
        {
            Id = restaurantId
        };
        
        var restaurant = new Restaurant
        {
            Id = restaurantId
        };

        // Tell the mock: "When GetByIdAsync(1) is called, return this restaurant"
        _restaurantRepositoryMock.Setup(r => r.GetByIdAsync(restaurantId))
            .ReturnsAsync(restaurant);

        // Tell the mock: "When Authorize is called, return true (authorized)"
        _restaurantAuthorizationServiceMock.Setup(a => a.Authorize(restaurant, 
            ResourceOperation.Update)).Returns(true);

        // Act: Call the handler
        await _handler.Handle(command, CancellationToken.None);

        // Assert: Verify correct methods were called
        
        // SaveChanges must be called exactly once - no more, no less
        _restaurantRepositoryMock.Verify(r => r.SaveChanges(), Times.Once);
        
        // Map must be called exactly once with the command and restaurant
        _mapperMock.Verify(m => m.Map(command, restaurant), Times.Once);
    }

    // ============================================================================
    // TEST 2: Error case - User is not authorized
    // ============================================================================
    
    /// <summary>
    /// SCENARIO: Unauthorized user tries to update a restaurant
    /// 
    /// EXPECTED BEHAVIOR:
    /// - Handler throws ForbidException
    /// - SaveChanges is NEVER called (prevents security breach)
    /// 
    /// WHY THIS MATTERS:
    /// Without this check, unauthorized users could modify restaurants.
    /// The Times.Never verification is a safety net.
    /// </summary>
    [Fact]
    public async Task Handle_WithUnauthorizedUser_ShouldThrowForbidException()
    {
        // Arrange: Set up test data
        var restaurantId = 1;
        var command = new UpdateRestaurantCommand() { Id = restaurantId };
        var restaurant = new Restaurant() { Id = restaurantId };

        // Restaurant exists in database
        _restaurantRepositoryMock.Setup(r => r.GetByIdAsync(restaurantId))
            .ReturnsAsync(restaurant);

        // But authorization check FAILS (user not authorized)
        _restaurantAuthorizationServiceMock.Setup(a => a.Authorize(restaurant, 
            ResourceOperation.Update)).Returns(false);

        // Act & Assert: Expect ForbidException to be thrown
        await _handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should()
            .ThrowAsync<ForbidException>();

        // CRITICAL: Verify SaveChanges was NEVER called
        // If authorization fails, we must not save anything
        _restaurantRepositoryMock.Verify(r => r.SaveChanges(), Times.Never);
    }

    // ============================================================================
    // TEST 3: Error case - Restaurant doesn't exist
    // ============================================================================
    
    /// <summary>
    /// SCENARIO: User tries to update a restaurant that doesn't exist
    /// 
    /// EXPECTED BEHAVIOR:
    /// - Handler throws NotFoundException
    /// - SaveChanges is NEVER called (prevents data corruption)
    /// 
    /// WHY THIS MATTERS:
    /// "Fail early" principle - if resource doesn't exist, stop immediately.
    /// Don't waste time authorizing, mapping, or saving non-existent data.
    /// </summary>
    [Fact]
    public async Task Handle_ForNonExistingRestaurant_ShouldThrowNotFoundException()
    {
        // Arrange: Set up test data
        var restaurantId = 999;  // Non-existent ID
        var command = new UpdateRestaurantCommand { Id = restaurantId };

        // Tell the mock: "When looking for restaurant 999, return null (not found)"
        _restaurantRepositoryMock.Setup(r => r.GetByIdAsync(restaurantId))
            .ReturnsAsync((Restaurant?)null);

        // Act & Assert: Expect NotFoundException to be thrown
        await _handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should()
            .ThrowAsync<NotFoundException>();

        // CRITICAL: Verify SaveChanges was NEVER called
        // If restaurant doesn't exist, we must not save anything
        _restaurantRepositoryMock.Verify(r => r.SaveChanges(), Times.Never);
    }
}
