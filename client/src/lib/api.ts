const API_BASE_URL = "http://localhost:7071/api";

export const carApi = {
  getCarStatus: async () => {
    const response = await fetch(`${API_BASE_URL}/car-status`);
    return response.json();
  },
  toggleCharging: async (isCharging: boolean) => {
    const response = await fetch(`${API_BASE_URL}/toggle-charging`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({ isCharging }),
    });
    return response.json();
  },
  scheduleCharge: async (scheduledCharge: Date) => {
    const response = await fetch(`${API_BASE_URL}/schedule-charge`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({ scheduledCharge }),
    });
    return response.json();
  },
}