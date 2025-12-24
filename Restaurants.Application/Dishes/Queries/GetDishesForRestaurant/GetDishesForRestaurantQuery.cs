using MediatR;
using Restaurants.Application.Dishes.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Restaurants.Application.Dishes.Queries.GetDishesForRestaurant
{
	public class GetDishesForRestaurantQuery(int restaurantId) : IRequest<IEnumerable<DishDto>>
	{
		public int RestaurantId{ get; } = restaurantId;
}
}
