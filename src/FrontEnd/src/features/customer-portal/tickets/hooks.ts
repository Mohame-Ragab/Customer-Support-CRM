import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import {
  getMyPortalTicket,
  listMyPortalTickets,
  submitPortalTicket,
  type ListMyPortalTicketsParams,
  type SubmitPortalTicketInput,
} from './api'

export function useMyPortalTickets(params: ListMyPortalTicketsParams) {
  return useQuery({
    queryKey: ['portal', 'tickets', 'list', params],
    queryFn: () => listMyPortalTickets(params),
    placeholderData: (previousData) => previousData,
  })
}

export function useMyPortalTicket(id: string) {
  return useQuery({
    queryKey: ['portal', 'tickets', 'detail', id],
    queryFn: () => getMyPortalTicket(id),
    enabled: Boolean(id),
  })
}

export function useSubmitPortalTicket() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: (input: SubmitPortalTicketInput) => submitPortalTicket(input),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['portal', 'tickets', 'list'] })
    },
  })
}
