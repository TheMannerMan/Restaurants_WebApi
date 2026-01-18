using Microsoft.EntityFrameworkCore;
using Restaurants.Domain.Constants;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Repositories;
using Restaurants.Infrastructure.Persistence;
using System.Globalization;
using System.Linq.Expressions;

namespace Restaurants.Infrastructure.Repositories;

internal class RestaurantsRepository(RestaurantsDbContext dbContext) : IRestaurantsRepository
{
    public async Task<int> Create(Restaurant entity)
    {
        dbContext.Restaurants.Add(entity);
        await dbContext.SaveChangesAsync();
        return entity.Id;
    }

    public async Task Delete(Restaurant entity)
    {
        dbContext.Remove(entity);
        await dbContext.SaveChangesAsync();

    }

    public async Task<IEnumerable<Restaurant>> GetAllAsync()
    {
        var restaurants = await dbContext.Restaurants.ToListAsync();
        return restaurants;
    }
    public async Task<(IEnumerable<Restaurant>, int)> GetAllMatchingAsync(string? searchPhrase,
        int pageSize,
        int pageNumber,
        string? sortBy,
        SortDirection sortDirection)
    {
        var searchPhraseToLower = searchPhrase?.ToLower();

        var baseQuery = dbContext.Restaurants
            .Where(r => searchPhrase == null || (r.Name.ToLower().Contains(searchPhraseToLower) ||
                                                r.Description.ToLower().Contains(searchPhraseToLower)));

        var totalCount = await baseQuery.CountAsync();

        if (sortBy != null)
        {
            var columnsSelectors = new Dictionary<string, Expression<Func<Restaurant, object>>>
            {
                { nameof(Restaurant.Name), r => r.Name },
                { nameof(Restaurant.Description), r => r.Description },
                { nameof(Restaurant.Category), r => r.Category },
            };
            var selectedColumn = columnsSelectors[sortBy];
            baseQuery = sortDirection == SortDirection.Ascending 
                ? baseQuery.OrderBy(selectedColumn)
                : baseQuery.OrderByDescending(selectedColumn);
        }

        var restaurants = await baseQuery
            .Skip(pageSize * (pageNumber - 1))
            .Take(pageSize)
            .ToListAsync();

        //Alternatives bellow:
        /*
         Alternative using EF.Functions.Like for case-insensitive search
        var restaurants = await dbContext.Restaurants
            .Where(r => searchPhrase == null || (EF.Functions.Like(r.Name, $"%{searchPhrase}%") ||
                                                EF.Functions.Like(r.Description, $"%{searchPhrase}%")))
            .ToListAsync();
         
        Altenrative for better better readablity:
        if (string.IsNullOrWhiteSpace(searchPhrase))
        return await dbContext.Restaurants.ToListAsync();

        var sp = searchPhrase!.ToLower();
        return await dbContext.Restaurants
            .Where(r => r.Name.ToLower().Contains(sp) || r.Description.ToLower().Contains(sp))
            .ToListAsync();

         */


        return (restaurants, totalCount);
    }


    public Task<Restaurant?> GetByIdAsync(int id)
    {
        var restaurant = dbContext.Restaurants
            .Include(r => r.Dishes)
            .FirstOrDefaultAsync(r => r.Id == id);

        return restaurant;
    }

    public async Task SaveChanges()
    {
        await dbContext.SaveChangesAsync();
    }
}
