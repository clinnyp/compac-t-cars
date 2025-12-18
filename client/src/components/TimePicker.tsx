import { Clock8Icon } from 'lucide-react'
import { Input } from '@/components/ui/input'

type Props = {
  time: string
  setTime: (time: string) => void
}

export default function TimePicker({ time, setTime }: Props) {
  return (
    <div className='w-full space-y-2'>
      <div className='relative w-full max-w-xs mx-auto'>
        <div className='text-muted-foreground pointer-events-none absolute inset-y-0 left-0 flex items-center justify-center pl-3 peer-disabled:opacity-50'>
          <Clock8Icon className='size-4' />
        </div>
        <Input
          type='time'
          id='time-picker'
          step={60}
          value={time}
          onChange={(e) => setTime(e.target.value)}
          className='w-full peer bg-background appearance-none pl-9 [&::-webkit-calendar-picker-indicator]:hidden [&::-webkit-calendar-picker-indicator]:appearance-none'
        />
      </div>
    </div>
  )
}

