import { useCarStatus } from "@/hooks/useCarStatus";
import { Label } from "@/components/ui/label";
import { Switch } from "@/components/ui/switch";
import { Button } from "@/components/ui/button";


export default function CardStatusCard() {
  const { data: carStatus, isLoading, error } = useCarStatus();

  return (
    <div className="flex justify-around items-center bg-gradient-to-r from-cyan-500/10 via-blue-500/10 to-purple-500/10 rounded-lg p-4 shadow-md">
      <div>
        <img
          src="/charging-car.svg"
          alt="Car"
          width={100}
          height={100}
        />
      </div>
      <div className="flex flex-col gap-1">
        <h1 className="text-lg font-bold">Simulated Car</h1>
        <p className="text-sm text-gray-600">
          {isLoading && "Loading..."}
          {error && `Error: ${error.message}`}
          {carStatus && `Battery: ${carStatus.batteryLevel}% | Charging: ${carStatus.isCharging ? 'Yes' : 'No'}`}
        </p>
        <Button variant="outline">
          Schedule Charge
        </Button>
      </div>
      <div>
        <Switch id="charging-status" />
        <Label className="text-sm text-gray-600 font-bold uppercase" htmlFor="charging-status">Charge</Label>
      </div>
    </div>
  )
}
