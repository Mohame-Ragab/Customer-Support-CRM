import { useQuery } from '@tanstack/react-query'
import { getPublishedArticle, listPublishedArticles } from '../api/knowledgeBaseApi'
import type { KnowledgeBaseContentListParams } from '../types'

export function usePublishedArticles(params: KnowledgeBaseContentListParams) {
  return useQuery({
    queryKey: ['knowledge-base', 'articles', 'list', params],
    queryFn: () => listPublishedArticles(params),
    placeholderData: (previousData) => previousData,
  })
}

export function usePublishedArticle(id: string) {
  return useQuery({
    queryKey: ['knowledge-base', 'articles', 'detail', id],
    queryFn: () => getPublishedArticle(id),
    enabled: Boolean(id),
  })
}
