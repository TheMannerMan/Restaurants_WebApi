using MediatR;
using Microsoft.AspNetCore.Mvc;
using Restaurants.Application.Restaurants.Commands.CreateRestaurant;
using Restaurants.Application.Restaurants.Commands.DeleteRestaurant;
using Restaurants.Application.Restaurants.Queries.GetAllRestaurants;
using Restaurants.Application.Restaurants.Queries.GetRestaurantById;
using Restaurants.Application.Restaurants.Commands.UpdateRestaurant;
using Restaurants.Application.Restaurants.Dtos;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Authorization;
using Restaurants.Domain.Constants;

namespace Restaurants.API.Controllers
{
	[ApiController]
	[Route("api/restaurants")]
	[Authorize] // Add authrozation for the whole controller and its endpoints.
	public class RestaurantsController(IMediator mediator) : ControllerBase
	{
		[HttpGet]
		[AllowAnonymous] //Allows anonymous access to this endpoint, bypassing the controller-level authorization requirement.
		//[Authorize]  - adds authorization for the specifik endpoint
		public async Task<ActionResult<IEnumerable<RestaurantDto>>> GetAll()
		{
			var restaurants = await mediator.Send(new GetAllRestaurantsQuery());
			return Ok(restaurants);

		}

		[HttpGet("{id}")]
		public async Task<ActionResult<RestaurantDto>> GetById(int id)
		{
			var restaurant = await mediator.Send(new GetRestaurantByIdQuery(id));
			return Ok(restaurant);
		}

		[HttpPost]
		[Authorize(Roles = UserRoles.Owner)]
		public async Task<IActionResult> CreateRestaurant(CreateRestaurantCommand command)
		{
			int id = await mediator.Send(command);
			return CreatedAtAction(nameof(GetById), new { id }, null);
		}

		[HttpDelete("{id}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> DeleteRestaurant([FromRoute] int id)
		{
			await mediator.Send(new DeleteRestaurantCommand(id));

			return NoContent();

		}

		[HttpPatch("{id}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> UpdateRestaurant([FromRoute] int id, [FromBody] UpdateRestaurantCommand command)
		{
			command.Id = id;
			await mediator.Send(command);

			return NoContent();

		}
	}
}
