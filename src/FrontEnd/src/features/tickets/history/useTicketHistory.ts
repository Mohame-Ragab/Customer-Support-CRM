import { useQuery } from '@tanstack/react-query'
import { getTicketHistory } from './api'

export function useTicketHistory(ticketId: string) {
  return useQuery({
    queryKey: ['tickets', ticketId, 'history'],
    queryFn: () => getTicketHistory(ticketId),
    enabled: Boolean(ticketId),
  })
}
