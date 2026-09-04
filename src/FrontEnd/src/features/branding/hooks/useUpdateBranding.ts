import { useMutation, useQueryClient } from '@tanstack/react-query'
import { updateBranding } from '../api/brandingApi'
import type { UpdateBrandingRequest } from '../types'

export function useUpdateBranding() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: (body: UpdateBrandingRequest) => updateBranding(body),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['branding'] })
    },
  })
}
