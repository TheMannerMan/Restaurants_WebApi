
using System.Diagnostics;

namespace Restaurants.API.Middlewares
{
	public class RequestTimeLoggingMiddleware(ILogger<RequestTimeLoggingMiddleware> logger) : IMiddleware
	{
		public async Task InvokeAsync(HttpContext context, RequestDelegate next)
		{
			var stopWatch = Stopwatch.StartNew();

			await next.Invoke(context);

			stopWatch.Stop();

			if (stopWatch.ElapsedMilliseconds > 4000)
			{
				logger.LogWarning("HTTP {Verb} with {Path} took {Duration} seconds to complete", 
					context.Request.Method, 
					context.Request.Path,
					stopWatch.ElapsedMilliseconds);
			}
		}
	}
}
