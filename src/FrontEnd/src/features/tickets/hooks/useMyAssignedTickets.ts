import { useQuery } from '@tanstack/react-query'
import { fetchTickets } from '../api/ticketsApi'

export function useMyAssignedTickets() {
  return useQuery({
    queryKey: ['tickets', 'assigned-to-me'],
    queryFn: () => fetchTickets({ assignedToMe: true, page: 1, pageSize: 100 }),
  })
}
