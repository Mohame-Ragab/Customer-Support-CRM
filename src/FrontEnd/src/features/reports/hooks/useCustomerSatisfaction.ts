import { useQuery } from '@tanstack/react-query'
import { getCustomerSatisfaction } from '../api/reportsApi'

export function useCustomerSatisfaction(from: string, to: string) {
  return useQuery({
    queryKey: ['reports', 'customer-satisfaction', { from, to }],
    queryFn: () => getCustomerSatisfaction(from, to),
    enabled: Boolean(from && to && from <= to),
  })
}
