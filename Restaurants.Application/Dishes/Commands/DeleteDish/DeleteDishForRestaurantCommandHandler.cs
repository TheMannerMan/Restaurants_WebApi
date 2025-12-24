using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Exceptions;
using Restaurants.Domain.Repositories;

namespace Restaurants.Application.Dishes.Commands.DeleteDish;

internal class DeleteDishForRestaurantCommandHandler(ILogger<DeleteDishForRestaurantCommandHandler> logger,
IRestaurantsRepository restaurantRepository,
IDishesRepository dishesRepository) : IRequestHandler<DeleteDishForRestaurantCommand>
{
	public async Task Handle(DeleteDishForRestaurantCommand request, CancellationToken cancellationToken)
	{
		logger.LogInformation($"Deleting dish with id {request.DishId} for restaurant with id {request.RestaurantId}");

		var restaurant = await restaurantRepository.GetByIdAsync(request.RestaurantId);
		if (restaurant == null ) throw new NotFoundException(nameof(Restaurant), request.RestaurantId.ToString());

		var dish = restaurant.Dishes.FirstOrDefault(p => p.Id == request.DishId);
		if (dish == null ) throw new NotFoundException(nameof(Dish), request.DishId.ToString());

		await dishesRepository.DeleteByIdAsync(dish);
	}
}
