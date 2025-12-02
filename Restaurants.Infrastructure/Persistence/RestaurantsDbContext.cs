using Microsoft.EntityFrameworkCore;
using Restaurants.Domain.Entities;

namespace Restaurants.Infrastructure.Persistence;

internal class RestaurantsDbContext(DbContextOptions<RestaurantsDbContext> options) : DbContext(options)
{
	public DbSet<Restaurant> Restaurants { get; set; }
	public DbSet<Dish> Dishes { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Restaurant>(entity =>
		{
			entity.OwnsOne(r => r.Address);
			entity.HasMany(r => r.Dishes)
				  .WithOne()
				  .HasForeignKey(d => d.RestaurantId);
		});
	}
}
