namespace Services;
using Microsoft.Azure.Devices;
using System.Text.Json.Nodes;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

public class CarService : ICarService
{
    private readonly ILogger<CarService> _logger;
    private readonly ServiceClient _serviceClient;
    private readonly RegistryManager _registryManager;

    public CarService(ILogger<CarService> logger, IConfiguration configuration)
    {
        var connectionString = configuration["IotHubConnectionString"];
        _logger = logger;
        _serviceClient = ServiceClient.CreateFromConnectionString(connectionString);
        _registryManager = RegistryManager.CreateFromConnectionString(connectionString);
    }

    public async Task<JsonNode> GetCarStatus()
    {
        var twin = await _registryManager.GetTwinAsync("simDevice");
        var reportedProperties = twin.Properties.Reported;

        var jsonReportedProperties = reportedProperties.ToJson();
        var carStatus = JsonNode.Parse(jsonReportedProperties);
        return carStatus;
    }

    public async Task ToggleCharging(bool isCharging)
    {
        var method = new CloudToDeviceMethod("ToggleCharging");
        method.SetPayloadJson(JsonSerializer.Serialize(isCharging));

        await _serviceClient.InvokeDeviceMethodAsync("simDevice", method);
    }
}