import { useQuery } from '@tanstack/react-query'
import { fetchTicket } from '../api/ticketsApi'

export function useTicket(ticketId: string) {
  return useQuery({
    queryKey: ['tickets', ticketId],
    queryFn: () => fetchTicket(ticketId),
    enabled: Boolean(ticketId),
  })
}
