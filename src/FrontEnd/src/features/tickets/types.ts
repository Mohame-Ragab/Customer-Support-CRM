// Backend has no global JsonStringEnumConverter (see F01's InteractionType for
// the same decision), so all ticket enums serialize as their numeric value.
// Keep these in lockstep with the backend Domain.Enums types.

export const TicketStatus = {
  New: 0,
  InProgress: 1,
  Resolved: 2,
  Closed: 3,
} as const
export type TicketStatus = (typeof TicketStatus)[keyof typeof TicketStatus]

export const TicketPriority = {
  Low: 1,
  Medium: 2,
  High: 3,
  Urgent: 4,
} as const
export type TicketPriority = (typeof TicketPriority)[keyof typeof TicketPriority]

// F03 communication-channels.
export const TicketChannel = {
  Manual: 1,
  Email: 2,
  LiveChat: 3,
  WebForm: 4,
  CustomerPortal: 5,
} as const
export type TicketChannel = (typeof TicketChannel)[keyof typeof TicketChannel]

export interface Ticket {
  id: string
  subject: string
  description: string | null
  status: TicketStatus
  customerId: string
  categoryId: string | null
  categoryCode: string | null
  priority: TicketPriority | null
  assignedAgentId: string | null
  assignedAt: string | null
  isEscalated: boolean
  escalatedAt: string | null
  channel: TicketChannel
  createdAt: string
  createdBy: string | null
  updatedAt: string | null
}

export interface TicketCategory {
  id: string
  code: string
  nameEn: string
  nameAr: string
}
