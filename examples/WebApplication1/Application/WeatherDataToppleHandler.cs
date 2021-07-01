using Microsoft.Extensions.Logging;
using Stower;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication1.Domain;

namespace WebApplication1.Application
{
    public class WeatherDataToppleHandler : IToppleHandler<WeatherData>
    {
        private readonly ILogger<WeatherDataToppleHandler> _logger;

        public WeatherDataToppleHandler(ILogger<WeatherDataToppleHandler> logger)
        {
            _logger = logger;
        }

        public async Task Handle(IEnumerable<WeatherData> items)
        {
            await Task.Delay(5000);
            foreach (var item in items)
            {
                _logger.LogInformation($"{item.Location} -> {item.Degree}");
            }
        }
    }
}
