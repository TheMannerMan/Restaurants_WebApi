using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Restaurants.Application.Restaurants.Dtos;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Repositories;

namespace Restaurants.Application.Restaurants.Commands.CreateRestaurant;

internal class CreateRestaurantCommandHandler(
	ILogger<CreateRestaurantCommandHandler> logger,
	IMapper mapper,
	IRestaurantsRepository restaurantsRepository) : IRequestHandler<CreateRestaurantCommand, int>
{
	public async Task<int> Handle(CreateRestaurantCommand request, CancellationToken cancellationToken)
	{
		logger.LogInformation("Creating a new restaurant");
		var restaurant = mapper.Map<Restaurant>(request);
		int id = await restaurantsRepository.Create(restaurant);
		return id;
	}
}
