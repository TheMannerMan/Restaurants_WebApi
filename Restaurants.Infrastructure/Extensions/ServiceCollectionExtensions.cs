using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Interfaces;
using Restaurants.Domain.Repositories;
using Restaurants.Infrastructure.Authorization;
using Restaurants.Infrastructure.Authorization.Requirements;
using Restaurants.Infrastructure.Authorization.Services;
using Restaurants.Infrastructure.Persistence;
using Restaurants.Infrastructure.Repositories;
using Restaurants.Infrastructure.Seeders;


namespace Restaurants.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment? environment = null)
    {

        var connectionString = configuration.GetConnectionString("RestaurantsDb");

        // Register DbContext with SQL Server provider
        services.AddDbContext<RestaurantsDbContext>(options => 
        {
            options.UseSqlServer(connectionString);
            // Only enable sensitive data logging in Development to avoid exposing sensitive data in production logs
            if (environment?.IsDevelopment() ?? false)
            {
                options.EnableSensitiveDataLogging();
            }
        });

        // AddIdentityApiEndpoints<User>() - Registers all Identity services in the DI container
        //   (UserManager, SignInManager, validators, token providers, authentication schemes)
        // AddRoles<IdentityRole>() - Enables role-based authorization by registering RoleManager<IdentityRole>
        //   and related role services for managing and assigning roles to users
        // AddEntityFrameworkStores<RestaurantsDbContext>() - Tells Identity to use your EF Core DbContext
        //   to save users in the database
        services.AddIdentityApiEndpoints<User>()
            .AddRoles<IdentityRole>()
            .AddClaimsPrincipalFactory<RestaurantsUserClaimsPrincipalFactory>()
            .AddEntityFrameworkStores<RestaurantsDbContext>();

        // Register application services
        services.AddScoped<IRestaurantSeeder, RestaurantSeeder>();
        services.AddScoped<IRestaurantsRepository, RestaurantsRepository>();
        services.AddScoped<IDishesRepository, DishesRepository>();
        services.AddScoped<IRestaurantAuthorizationService, RestaurantAuthorizationService>();
        services.AddAuthorizationBuilder()
            .AddPolicy(PolicyNames.HasNationality, builder => builder.RequireClaim(AppClaimTypes.Nationality, "German", "Swedish"))
            .AddPolicy(PolicyNames.AtLeast20,
                builder => builder.AddRequirements(new MinimumAgeRequirements(20)))
            .AddPolicy(PolicyNames.CreatedAtleast2Restaurants, 
                builder => builder.AddRequirements(new CreatedMultipleRestaurantsRequirement(2))
            );

        services.AddScoped<IAuthorizationHandler, MinimumAgeRequirementsHandler>();
        services.AddScoped<IAuthorizationHandler, CreatedMultipleRestaurantsRequirementHandler>();
    }
}
