using Restaurants.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Restaurants.Domain.Repositories;

public interface IDishesRepository
{
	//Task<IEnumerable<Dishes>> GetAllAsync();
	//Task<Restaurant?> GetByIdAsync(int id);
	Task<int> CreateAsync(Dish entity);

	Task DeleteByIdAsync(Dish entity);

	Task DeleteAsync(IEnumerable<Dish> entities);

	//Task Delete(Restaurant entity);
	//Task SaveChanges();
}
