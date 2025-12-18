import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { carApi } from '../lib/api';

export function useCarStatus() {
  return useQuery({
    queryKey: ['car-status'],
    queryFn: carApi.getCarStatus,
    refetchInterval: 2000,
  });
};

export function useToggleCharging() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (isCharging: boolean) => carApi.toggleCharging(isCharging),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['car-status'] });
    },
    onError: (error) => {
      console.error('Failed to toggle charging', error);
    },
  });
};

export function useScheduleCharge() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (scheduledCharge: Date) => carApi.scheduleCharge(scheduledCharge),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['car-status'] });
    },
    onError: (error) => {
      console.error('Failed to schedule charge', error);
    },
  });
};