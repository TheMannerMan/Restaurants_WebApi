

using AutoMapper;
using Microsoft.Extensions.Logging;
using Restaurants.Application.Restaurants.Dtos;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Repositories;

namespace Restaurants.Application.Restaurants;

public class RestaurantsService(IRestaurantsRepository restaurantsRepository,
	ILogger<RestaurantsService> logger,
	IMapper mapper ) : IRestaurantsService
{
	public async Task<IEnumerable<RestaurantDto>> GetAllRestaurants()
	{
		logger.LogInformation("Getting all restaurants");
		var restaurants = await restaurantsRepository.GetAllAsync();
		var restaurantsDto = mapper.Map<IEnumerable<RestaurantDto>>(restaurants);
		return restaurantsDto!;
	}

	public async Task<RestaurantDto?> GetById(int id)
	{
		logger.LogInformation("Getting restaurant by id: {Id}", id);
		var restaurant = await restaurantsRepository.GetByIdAsync(id);
		var restaurantDto = mapper.Map<RestaurantDto>(restaurant);
		return restaurantDto;
	}

	public async Task<int> Create(CreateRestaurantDto createRestaurantDto)
	{
		logger.LogInformation("Creating a new restaurant");
		var restaurant = mapper.Map<Restaurant>(createRestaurantDto);
		int id = await restaurantsRepository.Create(restaurant);
		return id;
	}
}
