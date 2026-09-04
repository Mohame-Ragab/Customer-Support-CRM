import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { listSystemSettings, updateSystemSetting } from '../api/systemConfigurationApi'
import type { UpdateSystemSettingRequest } from '../types'

export function useSystemSettings() {
  return useQuery({
    queryKey: ['system-settings'],
    queryFn: listSystemSettings,
  })
}

export function useUpdateSystemSetting() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: ({ key, body }: { key: string; body: UpdateSystemSettingRequest }) => updateSystemSetting(key, body),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['system-settings'] })
    },
  })
}
