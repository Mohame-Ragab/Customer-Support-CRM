import { apiClient } from '@/lib/api/apiClient'
import type { AgentPerformanceReport, CustomerSatisfactionReport, TicketReport } from '../types'

export async function getTicketReport(fromDate: string, toDate: string): Promise<TicketReport> {
  const response = await apiClient.get('/api/reports/tickets', { params: { fromDate, toDate } })
  return response.data
}

export interface AgentPerformanceParams {
  from: string
  to: string
  agentId?: string
}

export async function getAgentPerformance(
  params: AgentPerformanceParams,
): Promise<AgentPerformanceReport> {
  const response = await apiClient.get('/api/reports/agent-performance', { params })
  return response.data
}

export async function getCustomerSatisfaction(
  from: string,
  to: string,
): Promise<CustomerSatisfactionReport> {
  const response = await apiClient.get('/api/reports/customer-satisfaction', {
    params: { from, to },
  })
  return response.data
}
