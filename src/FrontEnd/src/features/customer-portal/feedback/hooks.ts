import { useMutation, useQueryClient } from '@tanstack/react-query'
import { submitFeedback, type SubmitFeedbackInput } from './api'

export function useSubmitFeedback(ticketId: string) {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: (input: SubmitFeedbackInput) => submitFeedback(ticketId, input),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['portal', 'tickets', 'detail', ticketId] })
    },
  })
}
