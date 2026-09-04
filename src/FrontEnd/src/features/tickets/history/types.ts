// Matches CustomerSupportCRM.Domain.Enums.TicketHistoryEventType exactly
// (numeric wire format - see features/tickets/types.ts for why).
export const TicketHistoryEventType = {
  Created: 1,
  CategoryChanged: 2,
  PriorityChanged: 3,
  AssignmentChanged: 4,
  StatusChanged: 5,
  Escalated: 6,
} as const
export type TicketHistoryEventType =
  (typeof TicketHistoryEventType)[keyof typeof TicketHistoryEventType]

export interface TicketHistoryEntry {
  id: string
  ticketId: string
  eventType: TicketHistoryEventType
  occurredAt: string // ISO
  actorUserId: string | null
  actorDisplayName: string | null
  oldValue: string | null
  newValue: string | null
  note: string | null
}
