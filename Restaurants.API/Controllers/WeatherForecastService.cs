namespace Restaurants.API.Controllers
{
	public class WeatherForecastService : IWeatherForecastService
	{
		private static readonly string[] Summaries =
		   [
			   "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
		   ];

		public IEnumerable<WeatherForecast> Get(int count, int minC, int maxC)
		{
			return Enumerable.Range(1, count).Select(index => new WeatherForecast
			{
				Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
				TemperatureC = Random.Shared.Next(minC, maxC),
				Summary = Summaries[Random.Shared.Next(Summaries.Length)]
			})
			.ToArray();

			//var filteredForecasts = forecasts.Where(forecast => forecast.TemperatureC >= minC && forecast.TemperatureC <= maxC).Take(count);
			//return filteredForecasts;
		}
	}
}
