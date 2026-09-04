import { useTranslation } from 'react-i18next'
import { Box, Divider, Typography } from '@mui/material'
import { LoadingState } from '@/components/ui/LoadingState'
import { ErrorState } from '@/components/ui/ErrorState'
import { EmptyState } from '@/components/ui/EmptyState'
import { normalizeApiError } from '@/lib/api/normalizeApiError'
import { useTicketInternalComments } from '../hooks/useTicketInternalComments'
import { InternalCommentList } from './InternalCommentList'
import { InternalCommentComposer } from './InternalCommentComposer'

interface InternalCommentThreadProps {
  ticketId: string
}

export function InternalCommentThread({ ticketId }: InternalCommentThreadProps) {
  const { t } = useTranslation()
  const { data: comments, isLoading, isError, error, refetch } = useTicketInternalComments(ticketId)

  return (
    <Box>
      <Typography variant="subtitle1" gutterBottom>
        {t('ticketInternalComments.title')}
      </Typography>

      {isLoading && <LoadingState />}

      {isError && (
        <ErrorState
          message={normalizeApiError(error).detail ?? t('ticketInternalComments.errorLoad')}
          onRetry={() => refetch()}
        />
      )}

      {!isLoading && !isError && (comments?.length ?? 0) === 0 && (
        <EmptyState title={t('ticketInternalComments.emptyState')} />
      )}

      {!isLoading && !isError && (comments?.length ?? 0) > 0 && (
        <InternalCommentList comments={comments ?? []} />
      )}

      <Divider sx={{ my: 2 }} />
      <InternalCommentComposer ticketId={ticketId} />
    </Box>
  )
}
