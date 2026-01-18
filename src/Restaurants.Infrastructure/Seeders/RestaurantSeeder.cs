using Microsoft.AspNetCore.Identity;
using Restaurants.Domain.Entities;
using Restaurants.Infrastructure.Persistence;
using Restaurants.Domain.Constants;

namespace Restaurants.Infrastructure.Seeders;

internal class RestaurantSeeder(RestaurantsDbContext dbContext) : IRestaurantSeeder
{
	public async Task Seed()
	{
		if (await dbContext.Database.CanConnectAsync())
		{
			if (!dbContext.Restaurants.Any())
			{
				var restaurants = GetRestaurants();
				dbContext.Restaurants.AddRange(restaurants);
				await dbContext.SaveChangesAsync();
			}

			if (!dbContext.Roles.Any())
			{
				var roles = GetRoles();
				dbContext.Roles.AddRange(roles);
				await dbContext.SaveChangesAsync();
			}
		}
	}

	private IEnumerable<IdentityRole> GetRoles()
	{
		List<IdentityRole> roles =
			[
				new(UserRoles.User)
				{
					NormalizedName = UserRoles.User.ToUpper()
				},
				new(UserRoles.Owner)
				{
					NormalizedName = UserRoles.Owner.ToUpper()
				},
				new(UserRoles.Admin)

				{
					NormalizedName = UserRoles.Admin.ToUpper()
				},
			];

		return roles;
	}

	private IEnumerable<Restaurant> GetRestaurants()
	{
		List<Restaurant> restaurants = [
			new(){
				Name = "KFC",
				Category = "Fast Food",
				Description = "Kentucky Fried Chicken is a fast food restaurant chain that specializes in fried chicken.",
				ContactEmail = "contact@kfc.com",
				HasDelivery = true,
				Dishes = [
					new(){
						Name = "Chicken Bucket",
						Description = "A bucket of fried chicken pieces.",
						Price = 19.99M
					},
					new(){
						Name = "Fries",
						Description = "Crispy golden fries.",
						Price = 2.99M
					}
				],
				Address = new(){
					City = "New York",
					Street = "123 Main St",
					PostalCode = "10001"
				}
			},
			new Restaurant(){
				Name = "McDonald's",
				Category = "Fast Food",
				Description = "McDonald's is a global fast food restaurant chain known for its burgers and fries.",
				ContactEmail = "contact@mcdonalds.com",
				HasDelivery = true,
				Address = new Address(){
					City = "Los Angeles",
					Street = "456 Elm St",
					PostalCode = "90001"
				},
				Dishes = [
					new Dish(){
						Name = "Big Mac",
						Description = "A classic double-decker burger with special sauce.",
						Price = 5.99M
					},
					new Dish(){
						Name = "Chicken Nuggets",
						Description = "Crispy chicken nuggets served with dipping sauce.",
						Price = 3.49M
					}
				]
			}
			];
		return restaurants;
	}
}
