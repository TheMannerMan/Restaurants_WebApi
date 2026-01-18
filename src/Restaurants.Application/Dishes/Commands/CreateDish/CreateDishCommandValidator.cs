using FluentValidation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Restaurants.Application.Dishes.Commands.Create;

public class CreateDishCommandValidator : AbstractValidator<CreateDishCommand>
{
	public CreateDishCommandValidator()
	{
		RuleFor(dish => dish.KiloCalories)
			.GreaterThanOrEqualTo(0)
			.WithMessage("KiloCalories must be non-negative number.");

		RuleFor(dish => dish.Price)
		.GreaterThanOrEqualTo(0)
		.WithMessage("Price must be non-negative number.");
	}
}
