# Stower
## Stacks incoming data and toppling it for batch operation when it comes to the stack limit



Microsoft.Extensions.DependencyInjection


```csharp 
services.AddStower(options =>
{
    options.AddStack<WeatherData>(Convert.ToInt32(_configuration["Stower:MaxStackLenght"]), Convert.ToInt32(_configuration["Stower:MaxWaitInSecond"]));
    options.AddStack<PositionData>(Convert.ToInt32(_configuration["Stower:MaxStackLenght"]), Convert.ToInt32(_configuration["Stower:MaxWaitInSecond"]));
}, typeof(WeatherDataToppleHandler).Assembly);
```


Sample injection and stack received data

```csharp 
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
    public async Task<IActionResult> AddNew(WeatherData item)
    {
        await _stower.Add(item);
        return Ok();
    }
}
```

Topple Handler -> It calls when stack count comes to the limit
```csharp 
public class ToppleHandler : IToppleHandler<WeatherData>
{
    public async Task Handle(List<WeatherData> items)
    {

    }
}
```
