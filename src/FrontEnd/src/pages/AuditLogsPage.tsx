import { useState } from 'react'
import { useTranslation } from 'react-i18next'
import { Container, Typography } from '@mui/material'
import { AuditLogsTable } from '@/features/audit/components/AuditLogsTable'

export function AuditLogsPage() {
  const { t } = useTranslation()
  const [page, setPage] = useState(0)
  const [pageSize, setPageSize] = useState(50)

  return (
    <Container maxWidth="lg" sx={{ py: 4 }}>
      <Typography variant="h4" component="h1" gutterBottom>
        {t('auditLogs.title')}
      </Typography>

      <AuditLogsTable
        page={page}
        pageSize={pageSize}
        onPageChange={setPage}
        onPageSizeChange={(size) => {
          setPageSize(size)
          setPage(0)
        }}
      />
    </Container>
  )
}
