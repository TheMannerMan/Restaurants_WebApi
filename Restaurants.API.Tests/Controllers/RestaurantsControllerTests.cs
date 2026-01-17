using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Restaurants.API.Controllers;
using Restaurants.Application.Restaurants.Dtos;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Repositories;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace Restaurants.API.Tests.Controllers;

// IClassFixture tells xUnit to create a WebApplicationFactory instance in memory (no separate server needed).
// This factory creates an in-memory version of the API that runs in the same process as the test.
public class RestaurantsControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    // This stores the in-memory API instance. When the test makes HTTP requests via CreateClient(),
    // they communicate with this in-memory API instead of a real running server.
    // This is much faster than starting a real server and makes HTTP calls over the network.
    private readonly WebApplicationFactory<Program> _factory;
    
    // Mock repository used for all tests. By default it returns null for any repository call.
    // Individual tests configure specific behaviors using Setup() to control the mock's responses.
    // This allows us to test the controller logic in isolation without a real database.
    private readonly Mock<IRestaurantsRepository> _restaurantsRepositoryMock = new();

    public RestaurantsControllerTests(WebApplicationFactory<Program> factory)
    {
        // Configure the in-memory API's dependency injection container for testing.
        // WithWebHostBuilder allows us to override the default services before the API starts.
        _factory = factory.WithWebHostBuilder(
            builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    // Replace the real IPolicyEvaluator with FakePolicyValidator to bypass authorization checks.
                    // This allows tests to focus on controller logic without needing valid JWT tokens or security policies.
                    services.AddSingleton<IPolicyEvaluator, FakePolicyValidator>();
                    
                    // Replace the real IRestaurantsRepository with our mock throughout the entire API.
                    // This ensures all endpoints use the mocked repository, giving us complete control over data responses.
                    // The mock's behavior is configured per test using Setup() calls.
                    services.Replace(ServiceDescriptor.Scoped(typeof(IRestaurantsRepository),
                                                    _ => _restaurantsRepositoryMock.Object));
                });
            });
    }

    [Fact]
    public async Task GetById_ForNonExistingId_ShouldReturnn404NotFound()
    {
        // Arrange
        var id = 1223;

        _restaurantsRepositoryMock.Setup(m => m.GetByIdAsync(id)).ReturnsAsync((Restaurant?)null);

        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync($"/api/restaurants/{id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetById_ForExistingId_ShouldReturnn200Ok()
    {
        // Arrange
        var id = 99;

        var restaurant = new Restaurant
        {
            Id = id,
            Name = "Test Restaurant",
            Description = "A test restaurant"
        };

        _restaurantsRepositoryMock.Setup(m => m.GetByIdAsync(id)).ReturnsAsync(restaurant);

        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync($"/api/restaurants/{id}");
        var restaurantDto = await response.Content.ReadFromJsonAsync<RestaurantDto>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        restaurantDto.Should().NotBeNull();
        restaurantDto.Name.Should().Be(restaurant.Name);
        restaurantDto.Description.Should().Be(restaurant.Description);

    }

    [Fact]
    public async Task GetAll_ForValidRequest_Returns200Ok()
    {
        // Arrange
        // CreateClient() returns an HttpClient connected to the in-memory API.
        // No real server is running - all requests stay in memory.
        var client = _factory.CreateClient();
        // Act
        var result = await client.GetAsync("/api/restaurants?pageNumber=1&pageSize=10");
        // Assert
        result.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetAll_ForInvalidRequest_Returns400BadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();
        // Act
        var result = await client.GetAsync("/api/restaurants");
        // Assert
        result.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }
}
