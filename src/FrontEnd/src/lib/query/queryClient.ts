import { QueryClient } from '@tanstack/react-query'

import { normalizeApiError } from '@/lib/api/normalizeApiError'

/**
 * Single QueryClient for the whole app. Feature query hooks (added once a
 * feature is implemented) should use this instance via useQuery/useMutation
 * rather than creating their own client.
 */
export const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      staleTime: 30_000,
      retry: (failureCount, error) => {
        // Never retry client errors (4xx) - retrying a 404/401/422 just
        // repeats the same failure. Retry network/5xx failures a couple of
        // times, since those can be transient.
        const status = normalizeApiError(error).status
        if (status !== null && status < 500) return false
        return failureCount < 2
      },
      refetchOnWindowFocus: false,
    },
    mutations: {
      retry: false,
    },
  },
})
