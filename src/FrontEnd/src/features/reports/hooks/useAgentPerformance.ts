import { useQuery } from '@tanstack/react-query'
import { getAgentPerformance, type AgentPerformanceParams } from '../api/reportsApi'

export function useAgentPerformance(query: AgentPerformanceParams) {
  return useQuery({
    queryKey: ['reports', 'agent-performance', query],
    queryFn: () => getAgentPerformance(query),
    enabled: Boolean(query.from && query.to && query.from <= query.to),
  })
}
