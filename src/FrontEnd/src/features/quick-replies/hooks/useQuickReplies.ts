import { useQuery } from '@tanstack/react-query'
import { listQuickReplies } from '../api/quickRepliesApi'

export function useQuickReplies(search?: string) {
  return useQuery({
    queryKey: ['quick-replies', { search }],
    queryFn: () => listQuickReplies(search),
  })
}
