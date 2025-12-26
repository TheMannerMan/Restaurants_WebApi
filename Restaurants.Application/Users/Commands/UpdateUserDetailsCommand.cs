using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Restaurants.Application.Users.Commands;

public class UpdateUserDetailsCommand : IRequest
{
	public DateOnly? DateOfBirth { get; set; }
	public string? Nationality { get; set; }
}
