using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Restaurants.API.Controllers;
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

    public RestaurantsControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
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
