import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { addCustomerNote, listCustomerNotes } from './api'

export function useCustomerNotes(customerId: string) {
  return useQuery({
    queryKey: ['customers', customerId, 'notes'],
    queryFn: () => listCustomerNotes(customerId),
    enabled: Boolean(customerId),
  })
}

export function useAddCustomerNote(customerId: string) {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: (content: string) => addCustomerNote(customerId, content),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['customers', customerId, 'notes'] })
    },
  })
}
