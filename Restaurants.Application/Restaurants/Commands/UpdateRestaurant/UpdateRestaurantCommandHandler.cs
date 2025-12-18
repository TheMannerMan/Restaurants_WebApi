using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Repositories;

namespace Restaurants.Application.Restaurants.Commands.UpdateRestaurant;

public class UpdateRestaurantCommandHandler(
	ILogger<UpdateRestaurantCommandHandler> logger,
	IMapper mapper,
	IRestaurantsRepository restaurantsRepository) : IRequestHandler<UpdateRestaurantCommand, bool>
{
	public async Task<bool> Handle(UpdateRestaurantCommand request, CancellationToken cancellationToken)
	{
		logger.LogInformation("Updating a restaurant with id {id}", request.Id);
		Restaurant? restaurant = await restaurantsRepository.GetByIdAsync(request.Id);
		if (restaurant is null)
			return false;

		mapper.Map(request, restaurant);

		//restaurant.Name = request.Name;
		//restaurant.Description = request.Description;
		//restaurant.HasDelivery = request.HasDelivery;

		await restaurantsRepository.SaveChanges();
		return true;
	}
}
