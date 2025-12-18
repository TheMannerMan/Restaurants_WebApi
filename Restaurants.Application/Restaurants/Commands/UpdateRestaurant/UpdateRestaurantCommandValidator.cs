using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Restaurants.Application.Restaurants.Commands.UpdateRestaurant;

public class UpdateRestaurantCommandValidator : AbstractValidator<UpdateRestaurantCommand>
{
	public UpdateRestaurantCommandValidator()
	{
		RuleFor(dto => dto.Name)
			.Length(3, 100);

	}
}
