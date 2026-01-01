using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Exceptions;
using Restaurants.Domain.Interfaces;
using Restaurants.Domain.Repositories;

namespace Restaurants.Application.Dishes.Commands.Create;

public class CreateDishCommandHandler(ILogger<CreateDishCommandHandler> logger,
	IRestaurantsRepository restaurantRepository,
	IDishesRepository dishesRepository,
	IMapper mapper, 
	IRestaurantAuthorizationService restaurantAuthorizationService) : IRequestHandler<CreateDishCommand, int>
{
	public async Task<int> Handle(CreateDishCommand request, CancellationToken cancellationToken)
	{
		logger.LogInformation("Creating a new dish: {@DishRequest}", request);
		var restaurant = await restaurantRepository.GetByIdAsync(request.RestaurantId);
		if (restaurant is null) throw new NotFoundException(nameof(Restaurant), request.RestaurantId.ToString());

        if (!restaurantAuthorizationService.Authorize(restaurant, Domain.Constants.ResourceOperation.Update))
            throw new ForbidException();

        var dish = mapper.Map<Dish>(request);

		return await dishesRepository.CreateAsync(dish);
	}
}
