using System.Text.Json.Nodes;

public interface ICarService
{
    Task<JsonNode> GetCarStatus();
    Task ToggleCharging(bool isCharging);
}