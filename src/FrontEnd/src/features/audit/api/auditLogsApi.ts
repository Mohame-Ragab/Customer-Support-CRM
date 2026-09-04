import { apiClient } from '@/lib/api/apiClient'
import type { AuditLogsResponse } from '../types'

export async function fetchAuditLogs(page: number, pageSize: number): Promise<AuditLogsResponse> {
  const response = await apiClient.get('/api/audit-logs', { params: { page, pageSize } })
  return response.data
}
