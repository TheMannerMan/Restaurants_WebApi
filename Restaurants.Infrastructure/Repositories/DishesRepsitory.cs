using Restaurants.Domain.Entities;
using Restaurants.Domain.Repositories;
using Restaurants.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace Restaurants.Infrastructure.Repositories;

internal class DishesRepsitory(RestaurantsDbContext dbContext) : IDishesRepository
{
	public async Task<int> Create(Dish entity)
	{
		dbContext.Dishes.Add(entity);
		await dbContext.SaveChangesAsync();
		return entity.Id;
	}
}
