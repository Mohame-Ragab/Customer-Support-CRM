import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import {
  createContent,
  deleteContent,
  getContent,
  listContent,
  updateContent,
} from '../api/knowledgeBaseApi'
import type {
  CreateKnowledgeBaseContentInput,
  KnowledgeBaseContentListParams,
  UpdateKnowledgeBaseContentInput,
} from '../types'

export function useContentList(params: KnowledgeBaseContentListParams) {
  return useQuery({
    queryKey: ['knowledge-base', 'content', 'list', params],
    queryFn: () => listContent(params),
    placeholderData: (previousData) => previousData,
  })
}

export function useContent(id: string) {
  return useQuery({
    queryKey: ['knowledge-base', 'content', 'detail', id],
    queryFn: () => getContent(id),
    enabled: Boolean(id),
  })
}

export function useCreateContent() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: (input: CreateKnowledgeBaseContentInput) => createContent(input),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['knowledge-base', 'content', 'list'] })
    },
  })
}

export function useUpdateContent(id: string) {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: (input: UpdateKnowledgeBaseContentInput) => updateContent(id, input),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['knowledge-base', 'content', 'list'] })
      queryClient.invalidateQueries({ queryKey: ['knowledge-base', 'content', 'detail', id] })
    },
  })
}

export function useDeleteContent() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: (id: string) => deleteContent(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['knowledge-base', 'content', 'list'] })
    },
  })
}
