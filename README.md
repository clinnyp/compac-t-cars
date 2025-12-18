# Compac-T-Cars

IoT application for monitoring and controlling electric vehicle charging via Azure IoT Hub. Features real-time battery monitoring, manual charging control, and scheduled charging.

## Architecture

```
React Client → Azure Functions → Azure IoT Hub → Car Simulator
```

- **Client**: React + TanStack Router/Query + Tailwind CSS
- **Server**: Azure Functions (C# .NET 8) managing IoT Hub communication
- **Simulator**: C# console app simulating an EV device

## Features

- Real-time battery level and charging status (3s polling)
- Manual charging toggle via UI
- Scheduled charging with automatic execution
- Device twin-based state synchronization

## Car Simulator

The simulator runs a continuous loop with **3-second ticks** that simulate battery behavior:

- **Initial State**: Random battery level between 20-51%
- **Charging Mode**: Battery increases by 1% every 3 seconds (up to 100%)
- **Discharging Mode**: Battery decreases by 1% every 3 seconds (down to 0%)
- **Scheduled Charge Check**: Every 3 seconds, checks device twin for scheduled charge time and executes when the current time matches (minute precision)

The simulator connects to IoT Hub via MQTT and reports state changes to the device twin, which the server reads and serves to the client.

## Quick Start

### Prerequisites
- Node.js 18+, npm
- .NET 8 SDK
- Azure IoT Hub with device

### Configuration

**Server** (`server/local.settings.json`):
```json
{
  "Values": {
    "IotHubConnectionString": "HostName=...;SharedAccessKeyName=...;SharedAccessKey=..."
  }
}
```

**Simulator** (User Secrets):
```bash
dotnet user-secrets set "DeviceConnectionString" "HostName=...;DeviceId=simDevice;SharedAccessKey=..."
```

### Run

```bash
# Terminal 1: Car Simulator
cd car-simulator && dotnet run

# Terminal 2: Azure Functions
cd server && func start

# Terminal 3: Client
cd client && npm install && npm run dev
```

Visit `http://localhost:3000`

## Project Structure

```
├── client/          # React frontend (TanStack Router/Query)
├── server/          # Azure Functions backend (C#)
└── car-simulator/  # Device simulator (C#)
```

## API Endpoints

- `GET /api/car-status` - Get car status
- `POST /api/toggle-charging` - Toggle charging state
- `POST /api/schedule-charge` - Schedule charge time

## Tech Stack

**Frontend**: React 19, TanStack Router/Query, Tailwind CSS 
**Backend**: .NET 8, Azure Functions, Azure IoT Hub SDK  
**Device**: .NET 8, Azure IoT Device SDK (MQTT)

## Data Flow

1. **Status Updates**: Device → IoT Hub → Server → Client (every 3s)
2. **Manual Control**: Client → Server → Device (direct method)
3. **Scheduled Charge**: Client → Server → Device Twin → Device (checks every 3s)
