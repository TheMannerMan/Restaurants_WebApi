using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.IdentityModel.Tokens;
using Restaurants.Application.Restaurants.Commands.CreateRestaurant;
using Restaurants.Application.Restaurants.Commands.UpdateRestaurant;
using Restaurants.Application.Restaurants.Dtos;
using Restaurants.Domain.Entities;
using Xunit;

namespace Restaurants.Application.Tests.Restaurants.Dtos;

public class RestaurantsProfileTests
{

    private IMapper _mapper;
    public RestaurantsProfileTests()
    {
       
        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<RestaurantsProfile>();
        }, NullLoggerFactory.Instance);

        _mapper = configuration.CreateMapper();
    }
    

    [Fact]
    public void CreateMap_FromRestaurantToRestaurantDto_MapsCorrectly()
    {
        // Arrange
        var restaurant = new Restaurant()
        {
            Id = 1,
            Name = "Test Restaurant",
            Description = "A test restaurant",
            Category = "Test Category",
            HasDelivery = true,
            ContactEmail = "test@test.com",
            ContactNumber = "1234567890",
            Address = new Address
            {
                Street = "123 Test St",
                City = "Test City",
                PostalCode = "12345"
            },
        };

        // act

        var restaurantDto = _mapper.Map<RestaurantDto>(restaurant);

        // Assert
        restaurantDto.Should().NotBeNull();
        restaurantDto.Id.Should().Be(restaurant.Id);
        restaurantDto.Name.Should().Be(restaurant.Name);
        restaurantDto.Description.Should().Be(restaurant.Description);
        restaurantDto.Category.Should().Be(restaurant.Category);
        restaurantDto.HasDelivery.Should().Be(restaurant.HasDelivery);
        restaurantDto.Street.Should().Be(restaurant.Address?.Street);
        restaurantDto.City.Should().Be(restaurant.Address?.City);
        restaurantDto.PostalCode.Should().Be(restaurant.Address?.PostalCode);
    }

    [Fact]
    public void CreateMap_FromRestaurantCommandToRestaurant_MapsCorrectly()
    {
        // Arrange
       

        var command = new CreateRestaurantCommand()
        {
            Name = "Test Restaurant",
            Description = "A test restaurant",
            Category = "Test Category",
            HasDelivery = true,
            ContactEmail = "test@test.com",
            ContactNumber = "1234567890",
            Street = "123 Test St",
            City = "Test City",
            PostalCode = "12345"
        };

        // act

        var restaurant = _mapper.Map<Restaurant>(command);

        // Assert
        restaurant.Should().NotBeNull();
        restaurant.Name.Should().Be(command.Name);
        restaurant.Description.Should().Be(command.Description);
        restaurant.Category.Should().Be(command.Category);
        restaurant.HasDelivery.Should().Be(command.HasDelivery);
        restaurant.ContactEmail.Should().Be(command.ContactEmail);
        restaurant.ContactNumber.Should().Be(command.ContactNumber);
        restaurant.Address.Should().NotBeNull();
        restaurant.Address.Street.Should().Be(command.Street);
        restaurant.Address.City.Should().Be(command.City);
        restaurant.Address.PostalCode.Should().Be(command.PostalCode);
    }

    [Fact]
    public void CreateMap_FromUpdateRestaurantCommandToRestaurant_MapsCorrectly()
    {
        // Arrange


        var command = new UpdateRestaurantCommand()
        {
           Id = 1,
            Name = "Test Restaurant",
            Description = "A test restaurant",
            HasDelivery = true
        };

        // act

        var restaurant = _mapper.Map<Restaurant>(command);

        // Assert
        restaurant.Should().NotBeNull();
        restaurant.Id.Should().Be(command.Id);
        restaurant.Name.Should().Be(command.Name);
        restaurant.Description.Should().Be(command.Description);
        restaurant.HasDelivery.Should().Be(command.HasDelivery);
    }

}

