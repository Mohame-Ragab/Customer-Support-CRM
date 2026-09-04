export interface QuickReplyTemplate {
  id: string
  name: string
  body: string
  createdAt: string
  updatedAt: string | null
}

export interface CreateQuickReplyInput {
  name: string
  body: string
}

export type UpdateQuickReplyInput = CreateQuickReplyInput
