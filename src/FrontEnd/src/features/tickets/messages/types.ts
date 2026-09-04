import { TicketChannel } from '../types'

// Backend has no global JsonStringEnumConverter - numeric wire values.
export const TicketMessageDirection = {
  Outbound: 1,
  Inbound: 2,
} as const
export type TicketMessageDirection =
  (typeof TicketMessageDirection)[keyof typeof TicketMessageDirection]

export const EmailDeliveryStatus = {
  Pending: 0,
  Sent: 1,
  Failed: 2,
  Received: 3,
} as const
export type EmailDeliveryStatus = (typeof EmailDeliveryStatus)[keyof typeof EmailDeliveryStatus]

export interface TicketMessage {
  id: string
  ticketId: string
  channel: TicketChannel
  direction: TicketMessageDirection
  deliveryStatus: EmailDeliveryStatus
  fromAddress: string
  toAddress: string
  cc: string | null
  subject: string
  bodyText: string
  bodyHtml: string | null
  failureReason: string | null
  sentByUserId: string | null
  createdAt: string
}
