export interface TicketInternalComment {
  id: string
  ticketId: string
  body: string
  authorId: string | null
  authorDisplayName: string | null
  createdAt: string
}
