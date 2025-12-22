using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Exceptions;
using Restaurants.Domain.Repositories;

namespace Restaurants.Application.Restaurants.Commands.UpdateRestaurant;

public class UpdateRestaurantCommandHandler(
	ILogger<UpdateRestaurantCommandHandler> logger,
	IMapper mapper,
	IRestaurantsRepository restaurantsRepository) : IRequestHandler<UpdateRestaurantCommand>
{
	public async Task Handle(UpdateRestaurantCommand request, CancellationToken cancellationToken)
	{
		logger.LogInformation("Updating a restaurant with id {id}, {@RestaurantId}", request.Id, request);
		Restaurant? restaurant = await restaurantsRepository.GetByIdAsync(request.Id);
		if (restaurant is null)
			throw new NotFoundException(nameof(Restaurant), request.Id.ToString());

		mapper.Map(request, restaurant);

		//restaurant.Name = request.Name;
		//restaurant.Description = request.Description;
		//restaurant.HasDelivery = request.HasDelivery;

		await restaurantsRepository.SaveChanges();
	}
}
