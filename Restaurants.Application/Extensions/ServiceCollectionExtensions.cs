using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Restaurants.Application.Restaurants;
using Restaurants.Application.Restaurants.Commands.CreateRestaurant;
using Restaurants.Application.Restaurants.Dtos;
using Restaurants.Application.User;
using Restaurants.Domain.Repositories;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;

namespace Restaurants.Application.Extensions;

public static class ServiceCollectionExtensions
{
	public static void AddApplication(this IServiceCollection services)
	{

		services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(ServiceCollectionExtensions).Assembly));
		services.AddAutoMapper(cfg => cfg.AddMaps(typeof(ServiceCollectionExtensions).Assembly));

		services.AddValidatorsFromAssemblyContaining<CreateRestaurantCommandValidator>();
		services.AddFluentValidationAutoValidation();

		services.AddScoped<IUserContext, UserContext>();

		// Registers IHttpContextAccessor as a singleton service in the DI container.
		// This is required by UserContext (which takes IHttpContextAccessor as a constructor parameter)
		// to access the current HTTP request context and extract user claims from HttpContext.User.
		// Without this registration, UserContext cannot be resolved and will throw a DI exception.
		services.AddHttpContextAccessor();
	}
}
