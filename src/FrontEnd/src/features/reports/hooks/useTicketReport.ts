import { useQuery } from '@tanstack/react-query'
import { getTicketReport } from '../api/reportsApi'

export function useTicketReport(fromDate: string, toDate: string) {
  return useQuery({
    queryKey: ['reports', 'tickets', { fromDate, toDate }],
    queryFn: () => getTicketReport(fromDate, toDate),
    enabled: Boolean(fromDate && toDate && fromDate <= toDate),
  })
}
