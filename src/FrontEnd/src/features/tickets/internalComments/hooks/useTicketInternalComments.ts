import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { addInternalComment, listInternalComments } from '../api/ticketInternalCommentsApi'

export function useTicketInternalComments(ticketId: string) {
  return useQuery({
    queryKey: ['tickets', ticketId, 'internal-comments'],
    queryFn: () => listInternalComments(ticketId),
    enabled: Boolean(ticketId),
  })
}

export function useAddTicketInternalComment(ticketId: string) {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: (body: string) => addInternalComment(ticketId, body),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['tickets', ticketId, 'internal-comments'] })
    },
  })
}
