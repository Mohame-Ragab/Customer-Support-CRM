import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { listCustomerAttachments, uploadCustomerAttachment } from './api'

export function useCustomerAttachments(customerId: string) {
  return useQuery({
    queryKey: ['customers', customerId, 'attachments'],
    queryFn: () => listCustomerAttachments(customerId),
    enabled: Boolean(customerId),
  })
}

export function useUploadCustomerAttachment(customerId: string) {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: (file: File) => uploadCustomerAttachment(customerId, file),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['customers', customerId, 'attachments'] })
    },
  })
}
