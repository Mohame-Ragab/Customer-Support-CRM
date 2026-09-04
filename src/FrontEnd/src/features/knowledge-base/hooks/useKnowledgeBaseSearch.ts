import { useQuery } from '@tanstack/react-query'
import { searchKnowledgeBase } from '../api/knowledgeBaseApi'

export function useKnowledgeBaseSearch(query: string, language?: string, page = 1, pageSize = 20) {
  const trimmed = query.trim()
  return useQuery({
    queryKey: ['kb-search', trimmed, language, page, pageSize],
    queryFn: () => searchKnowledgeBase({ query: trimmed, language, page, pageSize }),
    enabled: trimmed.length >= 2,
    placeholderData: (previousData) => previousData,
    staleTime: 30_000,
  })
}
