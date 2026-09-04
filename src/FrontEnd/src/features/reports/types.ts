export interface TicketReportBucket {
  key: string
  count: number
}

export interface TicketReport {
  fromDate: string
  toDate: string
  totalTickets: number
  byStatus: TicketReportBucket[]
  byCategory: TicketReportBucket[]
  byPriority: TicketReportBucket[]
}

export interface AgentPerformanceRow {
  agentId: string
  agentUserName: string
  ticketsAssigned: number
  ticketsResolved: number
  averageResolutionHours: number | null
}

export interface AgentPerformanceReport {
  from: string
  to: string
  rows: AgentPerformanceRow[]
}

export interface CustomerSatisfactionReport {
  periodStart: string
  periodEnd: string
  totalResponses: number
  averageRating: number
  satisfactionScorePercent: number
  ratingDistribution: Record<string, number>
}
