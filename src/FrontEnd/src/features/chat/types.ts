// Backend has no global JsonStringEnumConverter - numeric wire values.
export const ChatMessageKind = {
  Customer: 1,
  Agent: 2,
  System: 3,
} as const
export type ChatMessageKind = (typeof ChatMessageKind)[keyof typeof ChatMessageKind]

export interface ChatSession {
  id: string
  ticketId: string
  customerUserId: string
  assignedAgentUserId: string | null
  startedAtUtc: string
  endedAtUtc: string | null
}

export interface ChatMessage {
  id: string
  chatSessionId: string
  ticketId: string
  senderUserId: string | null
  kind: ChatMessageKind
  body: string
  sentAtUtc: string
}
