using Microsoft.AspNetCore.Mvc;
using Stower;
using System.Threading.Tasks;
using WebApplication1.Domain;

namespace WebApplication1.API
{
    [ApiController]
    [Route("weather")]
    public class WeatherController : ControllerBase
    {
        private readonly IStower _stower;

        public WeatherController(IStower stower)
        {
            _stower = stower;
        }
        [HttpPost]
        public IActionResult AddNew(WeatherData item)
        {
            Parallel.For(0, 1000, x =>
            {
                _stower.Add(item);
            });

            return Ok();
        }
    }
}
