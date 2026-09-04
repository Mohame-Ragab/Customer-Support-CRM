export interface Customer {
  id: string
  firstName: string
  lastName: string
  email: string
  phoneNumber: string | null
  companyName: string | null
  preferredLanguage: string | null
  notes: string | null
  createdAt: string
  createdBy: string | null
  updatedAt: string | null
}

export interface CreateCustomerInput {
  firstName: string
  lastName: string
  email: string
  phoneNumber?: string | null
  companyName?: string | null
  preferredLanguage?: string | null
  notes?: string | null
}

export type UpdateCustomerInput = CreateCustomerInput

export interface PagedResult<T> {
  items: T[]
  page: number
  pageSize: number
  totalCount: number
}
