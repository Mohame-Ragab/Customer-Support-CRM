// Backend has no global JsonStringEnumConverter, so InteractionType serializes
// as its numeric enum value - matches CustomerSupportCRM.Application.Features
// .Customers.InteractionHistory.InteractionType exactly (Ticket = 1,
// Communication = 2). Do not renumber without updating both sides.
export const InteractionType = {
  Ticket: 1,
  Communication: 2,
} as const

export type InteractionType = (typeof InteractionType)[keyof typeof InteractionType]

export interface CustomerInteractionDto {
  id: string
  type: InteractionType
  occurredAtUtc: string // ISO
  title: string
  summary?: string | null
  status?: string | null
  channel?: string | null
  sourceEntityId?: string | null
}

export interface CustomerInteractionHistoryResult {
  customerId: string
  items: CustomerInteractionDto[]
  totalCount: number
  page: number
  pageSize: number
}
