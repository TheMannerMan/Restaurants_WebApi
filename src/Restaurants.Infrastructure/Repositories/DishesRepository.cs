using Restaurants.Domain.Entities;
using Restaurants.Domain.Repositories;
using Restaurants.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace Restaurants.Infrastructure.Repositories;

internal class DishesRepository(RestaurantsDbContext dbContext) : IDishesRepository
{
	public async Task<int> CreateAsync(Dish entity)
	{
		dbContext.Dishes.Add(entity);
		await dbContext.SaveChangesAsync();
		return entity.Id;
	}

	public async Task DeleteByIdAsync(Dish entity)
	{
		dbContext.Dishes.Remove(entity);
		await dbContext.SaveChangesAsync();
	}

	public async Task DeleteAsync(IEnumerable<Dish> entities)
	{
		dbContext.Dishes.RemoveRange(entities);
		await dbContext.SaveChangesAsync();
	}
}
