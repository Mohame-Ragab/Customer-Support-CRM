import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { createCustomer, getCustomerById, listCustomers, updateCustomer } from '../api/customersApi'
import type { CreateCustomerInput, UpdateCustomerInput } from '../types/customer'

export function useCustomer(id: string) {
  return useQuery({
    queryKey: ['customers', 'detail', id],
    queryFn: () => getCustomerById(id),
    enabled: !!id,
  })
}

export function useCustomersList(page: number, pageSize: number) {
  return useQuery({
    queryKey: ['customers', 'list', { page, pageSize }],
    queryFn: () => listCustomers({ page, pageSize }),
    placeholderData: (previousData) => previousData,
  })
}

export function useCreateCustomer() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: (input: CreateCustomerInput) => createCustomer(input),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['customers', 'list'] })
    },
  })
}

export function useUpdateCustomer(id: string) {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: (input: UpdateCustomerInput) => updateCustomer(id, input),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['customers', 'list'] })
      queryClient.invalidateQueries({ queryKey: ['customers', 'detail', id] })
    },
  })
}
