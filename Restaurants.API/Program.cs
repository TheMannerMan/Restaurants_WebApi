using Restaurants.API.Extensions;
using Restaurants.API.Middlewares;
using Restaurants.Application.Extensions;
using Restaurants.Domain.Entities;
using Restaurants.Infrastructure.Extensions;
using Restaurants.Infrastructure.Seeders;
using Scalar.AspNetCore;
using Serilog;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.AddPresentation();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);


var app = builder.Build();

using var scope = app.Services.CreateScope();
var seeder = scope.ServiceProvider.GetRequiredService<IRestaurantSeeder>();
await seeder.Seed();

// Configure the HTTP request pipeline.
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseMiddleware<RequestTimeLoggingMiddleware>();

app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment()) // Scalar only visible during development.
{
	app.MapOpenApi();
	app.MapScalarApiReference(options =>
	{
		options
			.WithTitle("Restaurants API")
			.AddPreferredSecuritySchemes("Bearer")
			.AddHttpAuthentication("Bearer", auth =>
			{
				// Optional: Pre-fill token for testing (remove in production!)
				// auth.Token = "your-test-token-here";
			});
	});
}

app.UseHttpsRedirection();

// Maps HTTP endpoints for Identity (POST /register, POST /login, POST /refresh, POST /forgotPassword, etc.)
// These endpoints use the services registered in AddInfrastructure()
app.MapGroup("api/identity")
	.WithTags("Identity")
	.MapIdentityApi<User>();

app.UseAuthorization();

app.MapControllers();

app.Run();
