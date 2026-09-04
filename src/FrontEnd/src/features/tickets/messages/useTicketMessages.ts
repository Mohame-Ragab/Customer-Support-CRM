import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { listTicketMessages, sendEmailReply, type SendEmailReplyPayload } from './api'

export function useTicketMessages(ticketId: string) {
  return useQuery({
    queryKey: ['tickets', ticketId, 'messages'],
    queryFn: () => listTicketMessages(ticketId),
    enabled: Boolean(ticketId),
  })
}

export function useSendEmailReply(ticketId: string) {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: (payload: SendEmailReplyPayload) => sendEmailReply(ticketId, payload),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['tickets', ticketId, 'messages'] })
    },
  })
}
