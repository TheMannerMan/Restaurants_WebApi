using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;


namespace Restaurants.API.Controllers;

public class WeatherForecastRequest
{
	public int MinTemperatureC { get; set; }
	public int MaxTemperatureC { get; set; }
}

[ApiController]
[Route("api/[controller]")]
public class WeatherForecastController(IWeatherForecastService service, ILogger<WeatherForecastController> logger) : ControllerBase
{
	private readonly IWeatherForecastService _service = service;
	private readonly ILogger<WeatherForecastController> _logger = logger;


	[HttpPost]
	[Route("generate")]
	public IActionResult Generate([FromQuery]int count, [FromBody]WeatherForecastRequest request)
	{


		if (count < 0 || request.MaxTemperatureC < request.MinTemperatureC)
		{
			return BadRequest("Count has to be positive number, and max must be greater than min");
		}

		var result = _service.Get(count, request.MinTemperatureC, request.MaxTemperatureC);

		if (!result.Any() || result is null)
			return NotFound();

		return Ok(result);
	}



	//[HttpGet]
	//[Route("{take}/currentDay")]
	//public IActionResult Get([FromQuery] int max, [FromRoute] int take)
	//{
	//	var result = _service.Get().First();
	//	return Ok(result);
	//}

	/*[HttpGet]
	[Route("{take}/example")]
	public IEnumerable<WeatherForecast> Get([FromQuery] int max, [FromRoute] int take)
	{
		var result = _service.Get();
		return result;
	}

	[HttpGet]
	[Route("currentDay")]
	public WeatherForecast GetCurrentDayForecast()
	{
		var result = _service.Get().First();
		return result;
	} */

	//public string Hello([FromBody] string name) => $"Hello, {name}!";

}
