import { Button } from "@/components/ui/button"
import {
  Dialog,
  DialogClose,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog"
import TimePicker from "./TimePicker"
import { CalendarClock } from "lucide-react"
import { useState } from "react"
import { useScheduleCharge } from "@/hooks/useCarStatus"
import dayjs from 'dayjs'
import utc from 'dayjs/plugin/utc'

dayjs.extend(utc)


export default function TimePickerDialog() {
  const [time, setTime] = useState<string>('08:30')
  const [hours, minutes] = time.split(':').map(Number)
  const scheduleChargeMutation = useScheduleCharge()

  const localDateTime = dayjs().hour(hours).minute(minutes).second(0).millisecond(0)
  const utcTime = localDateTime.utc().toISOString()

  const handleScheduleCharge = () => {
    scheduleChargeMutation.mutate(new Date(utcTime))
  }

  return (
    <Dialog>
      <DialogTrigger asChild>
        <Button variant="outline">
          <CalendarClock size={16} />
          Schedule Charge
        </Button>
      </DialogTrigger>
      <DialogContent className="sm:max-w-md">
        <DialogHeader>
          <DialogTitle>Schedule Charge</DialogTitle>
        </DialogHeader>
        <DialogDescription>
          Choose a time to schedule a charge.
        </DialogDescription>
        <div className="flex items-center gap-2">
          <div className="grid flex-1 gap-2">
            <TimePicker time={time} setTime={setTime} />
          </div>
        </div>
        <DialogFooter className="sm:justify-end">
          <DialogClose asChild>
            <Button type="button" variant="default" onClick={handleScheduleCharge}>
              Save
            </Button>
          </DialogClose>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  )
}
