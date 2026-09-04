import { useParams, Link as RouterLink } from 'react-router-dom'
import { useTranslation } from 'react-i18next'
import { Box, Button, Card, CardContent, Container, Typography } from '@mui/material'
import { LoadingState } from '@/components/ui/LoadingState'
import { ErrorState } from '@/components/ui/ErrorState'
import { EmptyState } from '@/components/ui/EmptyState'
import { normalizeApiError } from '@/lib/api/normalizeApiError'
import { usePublishedArticle, ContentTypeChip } from '@/features/knowledge-base'

export function FaqDetailPage() {
  const { t } = useTranslation()
  const { id } = useParams<{ id: string }>()
  const { data: article, isLoading, isError, error, refetch } = usePublishedArticle(id ?? '')

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
            title={t('knowledgeBase.notFound')}
            action={
              <Button component={RouterLink} to="/portal/faqs">
                {t('customers.detail.backToList', { defaultValue: 'Back to list' })}
              </Button>
            }
          />
        </Container>
      )
    }
    return (
      <Container maxWidth="md" sx={{ py: 4 }}>
        <ErrorState
          title={t('errors.unexpected')}
          message={apiError.detail}
          onRetry={() => refetch()}
        />
      </Container>
    )
  }

  if (!article) {
    return null
  }

  return (
    <Container maxWidth="md" sx={{ py: 4 }}>
      <Box dir="auto">
        <Typography variant="h4" component="h1" gutterBottom>
          {article.title}
        </Typography>
        <ContentTypeChip type={article.type} />
      </Box>

      <Card sx={{ mt: 2 }}>
        <CardContent>
          {article.summary && (
            <Typography variant="subtitle1" color="text.secondary" gutterBottom dir="auto">
              {article.summary}
            </Typography>
          )}
          <Typography variant="body1" sx={{ whiteSpace: 'pre-wrap' }} dir="auto">
            {article.body}
          </Typography>
        </CardContent>
      </Card>
    </Container>
  )
}
