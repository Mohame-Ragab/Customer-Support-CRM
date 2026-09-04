export type UserRole = 'Admin' | 'Supervisor' | 'Manager' | 'Agent'

export const USER_ROLES: UserRole[] = ['Admin', 'Supervisor', 'Manager', 'Agent']

export interface User {
  id: string
  fullName: string | null
  email: string
  role: UserRole | string
  isActive: boolean
  createdAt: string
  modifiedAt: string | null
}

export interface CreateUserInput {
  fullName: string
  email: string
  role: UserRole
  initialPassword: string
}

export interface UpdateUserInput {
  fullName: string
  role: UserRole
  isActive: boolean
}

export interface PagedResult<T> {
  items: T[]
  page: number
  pageSize: number
  totalCount: number
}

export interface ListUsersParams {
  page?: number
  pageSize?: number
  search?: string
  role?: string
}
