import { useQuery } from '@tanstack/react-query'
import { getCustomerInteractionHistory } from './api'

export function useCustomerInteractionHistory(customerId: string, page: number, pageSize: number) {
  return useQuery({
    queryKey: ['customers', customerId, 'interaction-history', page, pageSize],
    queryFn: () => getCustomerInteractionHistory(customerId, page, pageSize),
    enabled: Boolean(customerId),
    placeholderData: (previousData) => previousData,
  })
}
