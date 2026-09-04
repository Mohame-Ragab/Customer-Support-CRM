import { apiClient } from '@/lib/api/apiClient'
import type {
  CreateKnowledgeBaseContentInput,
  KnowledgeBaseContent,
  KnowledgeBaseContentListItem,
  KnowledgeBaseContentListParams,
  KnowledgeBaseSearchParams,
  KnowledgeBaseSearchResult,
  PagedResult,
  UpdateKnowledgeBaseContentInput,
} from '../types'

export async function listContent(
  params: KnowledgeBaseContentListParams = {},
): Promise<PagedResult<KnowledgeBaseContentListItem>> {
  const response = await apiClient.get('/api/knowledge-base/content', { params })
  return response.data
}

export async function getContent(id: string): Promise<KnowledgeBaseContent> {
  const response = await apiClient.get(`/api/knowledge-base/content/${id}`)
  return response.data
}

export async function createContent(
  input: CreateKnowledgeBaseContentInput,
): Promise<KnowledgeBaseContent> {
  const response = await apiClient.post('/api/knowledge-base/content', input)
  return response.data
}

export async function updateContent(
  id: string,
  input: UpdateKnowledgeBaseContentInput,
): Promise<KnowledgeBaseContent> {
  const response = await apiClient.put(`/api/knowledge-base/content/${id}`, input)
  return response.data
}

export async function deleteContent(id: string): Promise<void> {
  await apiClient.delete(`/api/knowledge-base/content/${id}`)
}

export async function searchKnowledgeBase(
  params: KnowledgeBaseSearchParams,
): Promise<PagedResult<KnowledgeBaseSearchResult>> {
  const response = await apiClient.get('/api/knowledge-base/search', {
    params: {
      query: params.query,
      language: params.language,
      page: params.page,
      pageSize: params.pageSize,
    },
  })
  return response.data
}

// F08 customer-portal/access-faqs: public, published-only endpoints (distinct
// from listContent/getContent above, which hit the staff-only /content routes
// and would 403 for a Customer). Used for browsing without a search term.
export async function listPublishedArticles(
  params: Pick<KnowledgeBaseContentListParams, 'type' | 'language' | 'page' | 'pageSize'> = {},
): Promise<PagedResult<KnowledgeBaseContentListItem>> {
  const response = await apiClient.get('/api/knowledge-base/articles', { params })
  return response.data
}

export async function getPublishedArticle(id: string): Promise<KnowledgeBaseContent> {
  const response = await apiClient.get(`/api/knowledge-base/articles/${id}`)
  return response.data
}
