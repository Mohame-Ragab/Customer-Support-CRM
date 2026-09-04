import { useMutation, useQueryClient } from '@tanstack/react-query'
import { createQuickReply, deleteQuickReply, updateQuickReply } from '../api/quickRepliesApi'
import type { CreateQuickReplyInput, UpdateQuickReplyInput } from '../types/quickReply'

export function useQuickReplyMutations() {
  const queryClient = useQueryClient()
  const invalidate = () => queryClient.invalidateQueries({ queryKey: ['quick-replies'] })

  const createMutation = useMutation({
    mutationFn: (input: CreateQuickReplyInput) => createQuickReply(input),
    onSuccess: invalidate,
  })

  const updateMutation = useMutation({
    mutationFn: ({ id, input }: { id: string; input: UpdateQuickReplyInput }) =>
      updateQuickReply(id, input),
    onSuccess: invalidate,
  })

  const deleteMutation = useMutation({
    mutationFn: (id: string) => deleteQuickReply(id),
    onSuccess: invalidate,
  })

  return { createMutation, updateMutation, deleteMutation }
}
