using Microsoft.Extensions.Logging;
using Stower;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication1.Domain;

namespace WebApplication1.Application
{
    public class PositionDataToppleHandler : IToppleHandler<PositionData>
    {
        private readonly ILogger<PositionDataToppleHandler> _logger;

        public PositionDataToppleHandler(ILogger<PositionDataToppleHandler> logger)
        {
            _logger = logger;
        }

        public async Task Handle(IEnumerable<PositionData> items)
        {
            await Task.Delay(5000);
            foreach (var item in items)
            {
                _logger.LogInformation($"{item.Latitude} -> {item.Longitude}");
            }
        }
    }
}
