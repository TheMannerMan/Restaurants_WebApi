using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Restaurants.Application.Dishes.Commands.Create;

public class CreateDishCommand : IRequest<int>
{
	public string Name { get; set; } = default!;
	public string Description { get; set; } = default!;
	public decimal Price { get; set; }
	public int? KiloCalories { get; set; }

	public int RestaurantId { get; set; }
}
