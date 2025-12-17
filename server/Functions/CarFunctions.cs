using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services;


namespace Functions;

public class CarFunctions
{
    private readonly ILogger<CarFunctions> _logger;
    private readonly ICarService _carService;
    public record ChargingActionRequest(bool isCharging);

    public CarFunctions(ILogger<CarFunctions> logger, ICarService carService)
    {
        _logger = logger;
        _carService = carService;
    }

    [Function("HealthCheck")]
    public IActionResult CarHealthCheck([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "health-check")] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");
        return new OkObjectResult(new { message = "Ok." });
    }

    [Function("GetCarStatus")]
    public async Task<IActionResult> GetCarStatus([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "car-status")] HttpRequest req)
    {
        _logger.LogInformation("Getting car status...");
        
        var carStatus = await _carService.GetCarStatus();
        return new OkObjectResult(carStatus);
    }

    [Function("ToggleCharging")]
    public async Task<IActionResult> ToggleCharging([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "toggle-charging")] HttpRequest req)
    {
        _logger.LogInformation("Toggling charging...");
        var body = await req.ReadFromJsonAsync<ChargingActionRequest>();

        await _carService.ToggleCharging(body.isCharging); 
        return new OkObjectResult(new { message = $"{(body.isCharging ? "Started" : "Stopped")} charging." });
    }
}
