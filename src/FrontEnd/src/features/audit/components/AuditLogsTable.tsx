import { useTranslation } from 'react-i18next'
import { Box, Table, TableHead, TableBody, TableRow, TableCell, TablePagination } from '@mui/material'
import { LoadingState } from '@/components/ui/LoadingState'
import { ErrorState } from '@/components/ui/ErrorState'
import { EmptyState } from '@/components/ui/EmptyState'
import { normalizeApiError } from '@/lib/api/normalizeApiError'
import { useAuditLogsQuery } from '../hooks/useAuditLogsQuery'

interface AuditLogsTableProps {
  page: number
  pageSize: number
  onPageChange: (page: number) => void
  onPageSizeChange: (pageSize: number) => void
}

export function AuditLogsTable({ page, pageSize, onPageChange, onPageSizeChange }: AuditLogsTableProps) {
  const { t } = useTranslation()
  const { data, isLoading, isError, error, refetch } = useAuditLogsQuery(page + 1, pageSize)

  if (isLoading) {
    return <LoadingState />
  }

  if (isError) {
    const apiError = normalizeApiError(error)
    return (
      <ErrorState
        title={apiError.status === 403 ? t('auditLogs.errors.forbidden') : t('errors.unexpected')}
        message={apiError.detail}
        onRetry={() => refetch()}
      />
    )
  }

  if (!data || data.items.length === 0) {
    return <EmptyState title={t('auditLogs.empty')} />
  }

  return (
    <Box sx={{ overflowX: 'auto' }}>
      <Table>
        <TableHead>
          <TableRow>
            <TableCell>{t('auditLogs.columns.timestamp')}</TableCell>
            <TableCell>{t('auditLogs.columns.actor')}</TableCell>
            <TableCell>{t('auditLogs.columns.action')}</TableCell>
            <TableCell>{t('auditLogs.columns.entity')}</TableCell>
            <TableCell>{t('auditLogs.columns.summary')}</TableCell>
          </TableRow>
        </TableHead>
        <TableBody>
          {data.items.map((entry) => (
            <TableRow key={entry.id} hover>
              <TableCell>{new Date(entry.timestampUtc).toLocaleString()}</TableCell>
              <TableCell>{entry.performedByUserName ?? '—'}</TableCell>
              <TableCell>{entry.action}</TableCell>
              <TableCell>
                {entry.entityType ? `${entry.entityType}${entry.entityId ? ` #${entry.entityId}` : ''}` : '—'}
              </TableCell>
              <TableCell>{entry.summary ?? '—'}</TableCell>
            </TableRow>
          ))}
        </TableBody>
      </Table>
      <TablePagination
        component="div"
        count={data.totalCount}
        page={page}
        onPageChange={(_, newPage) => onPageChange(newPage)}
        rowsPerPage={pageSize}
        onRowsPerPageChange={(e) => onPageSizeChange(parseInt(e.target.value, 10))}
      />
    </Box>
  )
}
