export interface AuditLogEntry {
  id: string
  timestampUtc: string
  performedByUserId: string | null
  performedByUserName: string | null
  action: string
  entityType: string | null
  entityId: string | null
  summary: string | null
  metadataJson: string | null
}

export interface AuditLogsResponse {
  items: AuditLogEntry[]
  page: number
  pageSize: number
  totalCount: number
}
