// Backend has no global JsonStringEnumConverter - numeric wire values (see
// tickets/types.ts for the same, established decision).
export const ContentType = {
  Faq: 1,
  Article: 2,
  SolutionGuide: 3,
} as const
export type ContentType = (typeof ContentType)[keyof typeof ContentType]

export interface KnowledgeBaseContent {
  id: string
  title: string
  body: string
  summary: string | null
  type: ContentType
  language: string
  isPublished: boolean
  createdAt: string
  createdBy: string | null
  updatedAt: string | null
}

export interface KnowledgeBaseContentListItem {
  id: string
  title: string
  summary: string | null
  type: ContentType
  language: string
  isPublished: boolean
  createdAt: string
  updatedAt: string | null
}

export interface CreateKnowledgeBaseContentInput {
  title: string
  body: string
  summary?: string | null
  type: ContentType
  language?: string
  isPublished?: boolean
}

export type UpdateKnowledgeBaseContentInput = CreateKnowledgeBaseContentInput

export interface PagedResult<T> {
  items: T[]
  page: number
  pageSize: number
  totalCount: number
}

export interface KnowledgeBaseContentListParams {
  type?: ContentType
  language?: string
  isPublished?: boolean
  search?: string
  page?: number
  pageSize?: number
}

export interface KnowledgeBaseSearchResult {
  id: string
  title: string
  excerpt: string
  language: string
  score: number
  updatedAt: string
}

export interface KnowledgeBaseSearchParams {
  query: string
  language?: string
  page?: number
  pageSize?: number
}
