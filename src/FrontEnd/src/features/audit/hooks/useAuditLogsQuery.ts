import { useQuery } from '@tanstack/react-query'
import { fetchAuditLogs } from '../api/auditLogsApi'

export function useAuditLogsQuery(page: number, pageSize: number) {
  return useQuery({
    queryKey: ['audit-logs', page, pageSize],
    queryFn: () => fetchAuditLogs(page, pageSize),
  })
}
