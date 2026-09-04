import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { useTranslation } from 'react-i18next'
import {
  Container,
  Typography,
  Table,
  TableHead,
  TableBody,
  TableRow,
  TableCell,
  TablePagination,
  Box,
} from '@mui/material'
import { LoadingState } from '@/components/ui/LoadingState'
import { ErrorState } from '@/components/ui/ErrorState'
import { EmptyState } from '@/components/ui/EmptyState'
import { normalizeApiError } from '@/lib/api/normalizeApiError'
import { useCustomersList } from '@/features/customers/hooks/useCustomerQuery'

export function CustomersListPage() {
  const { t } = useTranslation()
  const navigate = useNavigate()
  const [page, setPage] = useState(0)
  const [pageSize, setPageSize] = useState(20)

  const { data, isLoading, isError, error, refetch } = useCustomersList(page + 1, pageSize)

  return (
    <Container maxWidth="lg" sx={{ py: 4 }}>
      <Typography variant="h4" component="h1" gutterBottom>
        {t('customers.list.title')}
      </Typography>

      {isLoading && <LoadingState />}

      {isError && (
        <ErrorState
          title={t('errors.unexpected')}
          message={normalizeApiError(error).detail}
          onRetry={() => refetch()}
        />
      )}

      {!isLoading && !isError && data && data.items.length === 0 && (
        <EmptyState title={t('customers.list.empty')} />
      )}

      {!isLoading && !isError && data && data.items.length > 0 && (
        <Box sx={{ overflowX: 'auto' }}>
          <Table>
            <TableHead>
              <TableRow>
                <TableCell>{t('customers.list.columns.name')}</TableCell>
                <TableCell>{t('customers.list.columns.email')}</TableCell>
                <TableCell>{t('customers.list.columns.phone')}</TableCell>
                <TableCell>{t('customers.list.columns.createdAt')}</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {data.items.map((customer) => (
                <TableRow
                  key={customer.id}
                  hover
                  onClick={() => navigate(`/customers/${customer.id}`)}
                  sx={{ cursor: 'pointer' }}
                >
                  <TableCell>
                    {customer.firstName} {customer.lastName}
                  </TableCell>
                  <TableCell>{customer.email}</TableCell>
                  <TableCell>{customer.phoneNumber ?? '—'}</TableCell>
                  <TableCell>{new Date(customer.createdAt).toLocaleDateString()}</TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
          <TablePagination
            component="div"
            count={data.totalCount}
            page={page}
            onPageChange={(_, newPage) => setPage(newPage)}
            rowsPerPage={pageSize}
            onRowsPerPageChange={(e) => {
              setPageSize(parseInt(e.target.value, 10))
              setPage(0)
            }}
          />
        </Box>
      )}
    </Container>
  )
}
