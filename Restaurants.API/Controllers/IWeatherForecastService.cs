
namespace Restaurants.API.Controllers
{
	public interface IWeatherForecastService
	{
		IEnumerable<WeatherForecast> Get(int count, int minC, int maxC);
	}
}