using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Repositories;
using Restaurants.Infrastructure.Persistence;
using Restaurants.Infrastructure.Repositories;
using Restaurants.Infrastructure.Seeders;


namespace Restaurants.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
	public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
	{
		
		var connectionString = configuration.GetConnectionString("RestaurantsDb");

		// Register DbContext with SQL Server provider
		services.AddDbContext<RestaurantsDbContext>(options => options.UseSqlServer(connectionString)
		.EnableSensitiveDataLogging()); // To see details like id values in logs

		// AddIdentityApiEndpoints<User>() - Registers all Identity services in the DI container
		//   (UserManager, SignInManager, validators, token providers, authentication schemes)
		// AddEntityFrameworkStores<RestaurantsDbContext>() - Tells Identity to use your EF Core DbContext
		//   to save users in the database
		services.AddIdentityApiEndpoints<User>().
			AddEntityFrameworkStores<RestaurantsDbContext>();

		// Register application services
		services.AddScoped<IRestaurantSeeder, RestaurantSeeder>();
		services.AddScoped<IRestaurantsRepository, RestaurantsRepository>();
		services.AddScoped<IDishesRepository, DishesRepository>();
	}
}
