import { useMutation, useQueryClient } from '@tanstack/react-query'
import {
  setTicketClassification,
  type SetTicketClassificationInput,
} from '../api/ticketClassificationApi'

export function useSetTicketClassificationMutation(ticketId: string) {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: (body: SetTicketClassificationInput) => setTicketClassification(ticketId, body),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['tickets', ticketId] })
    },
  })
}
