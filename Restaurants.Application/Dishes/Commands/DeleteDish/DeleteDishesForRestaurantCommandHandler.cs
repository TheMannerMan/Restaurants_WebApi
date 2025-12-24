using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Exceptions;
using Restaurants.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Restaurants.Application.Dishes.Commands.DeleteDish;

public class DeleteDishesForRestaurantCommandHandler(
	ILogger<DeleteDishesForRestaurantCommandHandler> logger,
	IRestaurantsRepository restaurantRepository,
	IDishesRepository dishesRepository)
	: IRequestHandler<DeleteDishesForRestaurantCommand>
{
	public async Task Handle(DeleteDishesForRestaurantCommand request, CancellationToken cancellationToken)
	{
		logger.LogWarning($"Deleting all dishes for restaurant with id {request.RestaurantId}");
		
		var restaurant = await restaurantRepository.GetByIdAsync(request.RestaurantId);
		if (restaurant == null) throw new NotFoundException(nameof(Restaurant), request.RestaurantId.ToString());

		await dishesRepository.DeleteAsync(restaurant.Dishes);

	}
}