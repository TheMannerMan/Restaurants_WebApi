using Microsoft.OpenApi;
using Restaurants.API.Middlewares;
using Serilog;

namespace Restaurants.API.Extensions;

public static class WebApplicationsBuilderExtensions
{
	public static void AddPresentation(this WebApplicationBuilder builder)
	{
		builder.Services.AddAuthentication();
		builder.Services.AddOpenApi(options =>
		{
			options.AddDocumentTransformer((document, context, cancellationToken) =>
			{
				// Add Bearer authentication security scheme to OpenAPI document
				document.Components ??= new OpenApiComponents();
				document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();

				document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
				{
					Type = SecuritySchemeType.Http,
					Scheme = "bearer",
					BearerFormat = "JWT",
					Description = "Enter your Bearer token"
				};

				return Task.CompletedTask;
			});
		}); // Scalar
		builder.Services.AddControllers();

		builder.Services.AddEndpointsApiExplorer();
		builder.Services.AddScoped<ErrorHandlingMiddleware>();
		builder.Services.AddScoped<RequestTimeLoggingMiddleware>();

		builder.Host.UseSerilog((context, configuration) =>
			configuration
				.ReadFrom.Configuration(context.Configuration));
		//.MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
		//.MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Information)
		//.WriteTo.File("Logs/Restaurant-API-.log", rollingInterval: RollingInterval.Day, rollOnFileSizeLimit: true)
		//.WriteTo.Console(outputTemplate: "[{Timestamp:dd-MM HH:mm:ss} {Level:u3}] |{SourceContext}| {NewLine}{Message:lj}{NewLine}{Exception}")
		//Flyttat till appsettings

	}
}
