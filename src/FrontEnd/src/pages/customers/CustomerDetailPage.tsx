import { useParams, Link } from 'react-router-dom'
import { useTranslation } from 'react-i18next'
import { Container, Typography, Card, CardContent, Stack, Box, Button } from '@mui/material'
import { LoadingState } from '@/components/ui/LoadingState'
import { ErrorState } from '@/components/ui/ErrorState'
import { EmptyState } from '@/components/ui/EmptyState'
import { normalizeApiError } from '@/lib/api/normalizeApiError'
import { useCustomer } from '@/features/customers/hooks/useCustomerQuery'
import { CustomerInteractionHistoryPanel } from '@/features/customers/interactionHistory/CustomerInteractionHistoryPanel'

export function CustomerDetailPage() {
  const { t } = useTranslation()
  const { id } = useParams<{ id: string }>()
  const { data: customer, isLoading, isError, error } = useCustomer(id ?? '')

  if (isLoading) {
    return (
      <Container maxWidth="md" sx={{ py: 4 }}>
        <LoadingState />
      </Container>
    )
  }

  if (isError) {
    const apiError = normalizeApiError(error)
    if (apiError.status === 404) {
      return (
        <Container maxWidth="md" sx={{ py: 4 }}>
          <EmptyState
            title={t('customers.detail.notFound')}
            action={
              <Button component={Link} to="/customers">
                {t('customers.detail.backToList', { defaultValue: 'Back to customers' })}
              </Button>
            }
          />
        </Container>
      )
    }

    return (
      <Container maxWidth="md" sx={{ py: 4 }}>
        <ErrorState title={t('errors.unexpected')} message={apiError.detail} />
      </Container>
    )
  }

  if (!customer) {
    return null
  }

  return (
    <Container maxWidth="md" sx={{ py: 4 }}>
      <Typography variant="h4" component="h1" gutterBottom>
        {t('customers.detail.title')}
      </Typography>

      <Stack spacing={3}>
        <Card>
          <CardContent>
            <Typography variant="subtitle1" gutterBottom>
              {t('customers.detail.sections.profile')}
            </Typography>
            <Typography variant="h6">
              {customer.firstName} {customer.lastName}
            </Typography>
            {customer.companyName && (
              <Typography variant="body2" color="text.secondary">
                {customer.companyName}
              </Typography>
            )}

            <Box sx={{ mt: 2 }}>
              <Typography variant="subtitle1" gutterBottom>
                {t('customers.detail.sections.contact')}
              </Typography>
              <Typography variant="body2">{customer.email}</Typography>
              {customer.phoneNumber && (
                <Typography variant="body2">{customer.phoneNumber}</Typography>
              )}
              {customer.preferredLanguage && (
                <Typography variant="body2" color="text.secondary">
                  {customer.preferredLanguage === 'ar' ? 'العربية' : 'English'}
                </Typography>
              )}
            </Box>
          </CardContent>
        </Card>

        <Card>
          <CardContent>
            <CustomerInteractionHistoryPanel customerId={customer.id} />
          </CardContent>
        </Card>
      </Stack>
    </Container>
  )
}
