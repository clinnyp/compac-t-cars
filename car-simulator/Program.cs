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

// Initialize reported properties
var reportedProperties = new TwinCollection();
var random = new Random();
var _batteryLevel = random.Next(20, 51);
var _isCharging = false;

reportedProperties["batteryLevel"] = _batteryLevel;
reportedProperties["isCharging"] = _isCharging;
await deviceClient.UpdateReportedPropertiesAsync(reportedProperties);
await deviceClient.SetMethodHandlerAsync("ToggleCharging", ToggleCharging, null);


// Registering callback for scheduled charging
DateTime? _scheduledChargeTime = null;
await deviceClient.SetDesiredPropertyUpdateCallbackAsync(ScheduledChargeCallback, null);

while (true) {
  await Task.Delay(3000);

  // Schedule Charing Simulation
  if (_scheduledChargeTime.HasValue)
  {
    var now = DateTime.UtcNow;
    if (now.Date == _scheduledChargeTime.Value.Date &&
        now.Hour == _scheduledChargeTime.Value.Hour &&
        now.Minute == _scheduledChargeTime.Value.Minute &&
    !_isCharging) 
    {
      _isCharging = true;
      reportedProperties["isCharging"] = _isCharging;
      await deviceClient.UpdateReportedPropertiesAsync(reportedProperties);
      _scheduledChargeTime = null;
      Console.WriteLine("Starting scheduled charging");
    }
  }
  
  // Charging simulation
  if (_isCharging && _batteryLevel < 100) {
    _batteryLevel++;
    reportedProperties["batteryLevel"] = _batteryLevel;
    await deviceClient.UpdateReportedPropertiesAsync(reportedProperties);
    Console.WriteLine($"Battery level: {_batteryLevel}%");
  } 

  // Discharging simulation
  if (!_isCharging && _batteryLevel > 0) {
    _batteryLevel--;
    reportedProperties["batteryLevel"] = _batteryLevel;
    await deviceClient.UpdateReportedPropertiesAsync(reportedProperties);
    Console.WriteLine($"Battery level: {_batteryLevel}%");
  }  
}

Task ScheduledChargeCallback(TwinCollection desiredProperties, object userContext)
{
  if (desiredProperties.Contains("scheduleCharge"))
  {
    var timeString = desiredProperties["scheduleCharge"]?.ToString();
    if (DateTime.TryParse(timeString, out DateTime scheduledCharge))
    {
      scheduledCharge = scheduledCharge.ToUniversalTime();
      _scheduledChargeTime = scheduledCharge;
      Console.WriteLine($"Scheduled charge time: {_scheduledChargeTime.Value.ToString("HH:mm")}");
    }
  }
  else 
  {
    _scheduledChargeTime = null;
    Console.WriteLine("Scheduled charge time cleared");
  }
  return Task.CompletedTask;
}

async Task<MethodResponse> ToggleCharging(MethodRequest methodRequest, object userContext) {
  var isCharging = JsonSerializer.Deserialize<bool>(methodRequest.DataAsJson);
  _isCharging = isCharging;
  reportedProperties["isCharging"] = _isCharging;
  await deviceClient.UpdateReportedPropertiesAsync(reportedProperties);
  Console.WriteLine($"{(_isCharging ? "Started" : "Stopped")} charging.");
  return new MethodResponse(200);
}
