import { useQuery } from '@tanstack/react-query'
import { fetchTicketCategories } from '../api/ticketClassificationApi'

export function useTicketCategoriesQuery() {
  return useQuery({
    queryKey: ['ticket-categories'],
    queryFn: fetchTicketCategories,
    staleTime: 5 * 60_000,
  })
}
