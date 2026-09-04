import { useState } from 'react'
import { useTranslation } from 'react-i18next'
import { Box, Typography, List, ListItem, ListItemText, Chip, Stack, Button } from '@mui/material'
import { LoadingState } from '@/components/ui/LoadingState'
import { ErrorState } from '@/components/ui/ErrorState'
import { EmptyState } from '@/components/ui/EmptyState'
import { normalizeApiError } from '@/lib/api/normalizeApiError'
import { useCustomerInteractionHistory } from './useCustomerInteractionHistory'
import { InteractionType } from './types'

interface CustomerInteractionHistoryPanelProps {
  customerId: string
}

export function CustomerInteractionHistoryPanel({
  customerId,
}: CustomerInteractionHistoryPanelProps) {
  const { t } = useTranslation()
  const [page, setPage] = useState(1)
  const pageSize = 20

  const { data, isLoading, isError, error, refetch } = useCustomerInteractionHistory(
    customerId,
    page,
    pageSize,
  )

  return (
    <Box>
      <Typography variant="h6" gutterBottom>
        {t('customers.interactionHistory.title')}
      </Typography>

      {isLoading && <LoadingState />}

      {isError && (
        <ErrorState
          title={t('customers.interactionHistory.error')}
          message={normalizeApiError(error).detail}
          onRetry={() => refetch()}
        />
      )}

      {!isLoading && !isError && data && data.items.length === 0 && (
        <EmptyState title={t('customers.interactionHistory.empty')} />
      )}

      {!isLoading && !isError && data && data.items.length > 0 && (
        <>
          <List>
            {data.items.map((item) => (
              <ListItem key={item.id} divider alignItems="flex-start">
                <ListItemText
                  primary={
                    <Stack direction="row" spacing={1} sx={{ alignItems: 'center' }}>
                      <Chip
                        size="small"
                        label={
                          item.type === InteractionType.Ticket
                            ? t('customers.interactionHistory.type.ticket')
                            : t('customers.interactionHistory.type.communication')
                        }
                      />
                      <Typography component="span" variant="subtitle2">
                        {item.title}
                      </Typography>
                      {item.status && <Chip size="small" variant="outlined" label={item.status} />}
                      {item.channel && (
                        <Chip size="small" variant="outlined" label={item.channel} />
                      )}
                    </Stack>
                  }
                  secondary={
                    <>
                      <Typography
                        component="span"
                        variant="caption"
                        color="text.secondary"
                        sx={{ display: 'block' }}
                      >
                        {new Date(item.occurredAtUtc).toLocaleString()}
                      </Typography>
                      {item.summary && (
                        <Typography component="span" variant="body2" color="text.secondary">
                          {item.summary}
                        </Typography>
                      )}
                    </>
                  }
                />
              </ListItem>
            ))}
          </List>

          {data.totalCount > page * pageSize && (
            <Button onClick={() => setPage((p) => p + 1)}>
              {t('customers.interactionHistory.loadMore')}
            </Button>
          )}
        </>
      )}
    </Box>
  )
}
