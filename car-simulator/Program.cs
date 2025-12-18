using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

var config = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .AddEnvironmentVariables()
    .Build();

var connectionString = config["DeviceConnectionString"];

using var deviceClient = DeviceClient.CreateFromConnectionString(connectionString, TransportType.Mqtt);
await deviceClient.OpenAsync();
Console.WriteLine("Connected!");

var reportedProperties = new TwinCollection();
var random = new Random();
var _batteryLevel = random.Next(20, 51);
var _isCharging = false;

reportedProperties["batteryLevel"] = _batteryLevel;
reportedProperties["isCharging"] = _isCharging;
await deviceClient.UpdateReportedPropertiesAsync(reportedProperties);
await deviceClient.SetMethodHandlerAsync("ToggleCharging", ToggleCharging, null);

while (true) {
  await Task.Delay(3000);

  var twin = await deviceClient.GetTwinAsync();
  var desiredProperties = twin.Properties.Desired;
  
  if (desiredProperties.Contains("scheduleCharge"))
  {
    var timeString = desiredProperties["scheduleCharge"]?.ToString();

    if (DateTime.TryParse(timeString, out DateTime scheduledCharge))
    {
      scheduledCharge = scheduledCharge.ToUniversalTime();
      var now = DateTime.UtcNow;

      if (scheduledCharge.Date == now.Date && 
          scheduledCharge.Hour == now.Hour && 
          scheduledCharge.Minute == now.Minute && 
          !_isCharging)
      {
        _isCharging = true;
        reportedProperties["isCharging"] = _isCharging;
        await deviceClient.UpdateReportedPropertiesAsync(reportedProperties);
        Console.WriteLine("Starting scheduled charging");
      }
    }
  } 
 
  if (_isCharging && _batteryLevel < 100) {
    _batteryLevel++;
    reportedProperties["batteryLevel"] = _batteryLevel;
    await deviceClient.UpdateReportedPropertiesAsync(reportedProperties);
    Console.WriteLine($"Battery level: {_batteryLevel}%");
  } 
  
  if (!_isCharging && _batteryLevel > 0) {
    _batteryLevel--;
    reportedProperties["batteryLevel"] = _batteryLevel;
    await deviceClient.UpdateReportedPropertiesAsync(reportedProperties);
    Console.WriteLine($"Battery level: {_batteryLevel}%");
  }  
}

async Task<MethodResponse> ToggleCharging(MethodRequest methodRequest, object userContext) {
  var isCharging = JsonSerializer.Deserialize<bool>(methodRequest.DataAsJson);
  _isCharging = isCharging;
  reportedProperties["isCharging"] = _isCharging;
  await deviceClient.UpdateReportedPropertiesAsync(reportedProperties);
  Console.WriteLine($"{(_isCharging ? "Started" : "Stopped")} charging.");
  return new MethodResponse(200);
}
