/** Cross-cutting shapes with no owning feature. No CRM domain models here. */

/** Shape for a future paginated list endpoint; not tied to any specific entity. */
export interface PaginatedResult<T> {
  items: T[]
  totalCount: number
  pageNumber: number
  pageSize: number
}

export type Nullable<T> = T | null

export type Direction = 'ltr' | 'rtl'
