import { useState } from 'react'
import { useNavigate, useParams, Link as RouterLink } from 'react-router-dom'
import { useTranslation } from 'react-i18next'
import { Alert, Box, Button, Card, CardContent, Snackbar, Stack, Typography } from '@mui/material'
import { LoadingState } from '@/components/ui/LoadingState'
import { ErrorState } from '@/components/ui/ErrorState'
import { EmptyState } from '@/components/ui/EmptyState'
import { normalizeApiError } from '@/lib/api/normalizeApiError'
import {
  useContent,
  useDeleteContent,
  useUpdateContent,
} from '@/features/knowledge-base/hooks/useKnowledgeBaseContent'
import {
  ContentFormDialog,
  type ContentFormValues,
} from '@/features/knowledge-base/components/ContentFormDialog'
import { ContentTypeChip } from '@/features/knowledge-base/components/ContentTypeChip'

export function ContentDetailPage() {
  const { t } = useTranslation()
  const navigate = useNavigate()
  const { id } = useParams<{ id: string }>()
  const { data: content, isLoading, isError, error, refetch } = useContent(id ?? '')

  const updateMutation = useUpdateContent(id ?? '')
  const deleteMutation = useDeleteContent()
  const [dialogOpen, setDialogOpen] = useState(false)
  const [formError, setFormError] = useState<string | null>(null)
  const [snackbarMessage, setSnackbarMessage] = useState<string | null>(null)

  const handleSubmit = (values: ContentFormValues) => {
    setFormError(null)
    updateMutation
      .mutateAsync({
        title: values.title,
        body: values.body,
        summary: values.summary || null,
        type: values.type,
        language: values.language,
        isPublished: values.isPublished,
      })
      .then(() => setDialogOpen(false))
      .catch((err) => setFormError(normalizeApiError(err).detail ?? t('errors.unexpected')))
  }

  const handleDelete = () => {
    if (!content) return
    if (!window.confirm(t('knowledgeBase.confirmDelete', { defaultValue: 'Delete this item?' })))
      return
    deleteMutation.mutate(content.id, {
      onSuccess: () => navigate('/knowledge-base'),
      onError: (err) => setSnackbarMessage(normalizeApiError(err).detail ?? t('errors.unexpected')),
    })
  }

  if (isLoading) {
    return (
      <Box sx={{ py: 4, px: 2 }}>
        <LoadingState />
      </Box>
    )
  }

  if (isError) {
    const apiError = normalizeApiError(error)
    if (apiError.status === 404) {
      return (
        <Box sx={{ py: 4, px: 2 }}>
          <EmptyState
            title={t('knowledgeBase.notFound', { defaultValue: 'Content not found' })}
            action={
              <Button component={RouterLink} to="/knowledge-base">
                {t('customers.detail.backToList', { defaultValue: 'Back to list' })}
              </Button>
            }
          />
        </Box>
      )
    }
    return (
      <Box sx={{ py: 4, px: 2 }}>
        <ErrorState
          title={t('errors.unexpected')}
          message={apiError.detail}
          onRetry={() => refetch()}
        />
      </Box>
    )
  }

  if (!content) {
    return null
  }

  return (
    <Box sx={{ py: 4, px: 2, maxWidth: 900, mx: 'auto' }}>
      <Stack
        direction="row"
        sx={{ justifyContent: 'space-between', alignItems: 'flex-start', mb: 2 }}
      >
        <Box dir="auto">
          <Typography variant="h4" component="h1" gutterBottom>
            {content.title}
          </Typography>
          <Stack direction="row" spacing={1} sx={{ alignItems: 'center' }}>
            <ContentTypeChip type={content.type} />
            <Typography variant="body2" color="text.secondary">
              {t(`knowledgeBase.language.${content.language}`, { defaultValue: content.language })}
            </Typography>
            <Typography variant="body2" color="text.secondary">
              {content.isPublished
                ? t('knowledgeBase.publishedYes', { defaultValue: 'Published' })
                : t('knowledgeBase.publishedNo', { defaultValue: 'Draft' })}
            </Typography>
          </Stack>
        </Box>
        <Stack direction="row" spacing={1}>
          <Button
            variant="outlined"
            onClick={() => {
              setFormError(null)
              setDialogOpen(true)
            }}
          >
            {t('users.actions.edit', { defaultValue: 'Edit' })}
          </Button>
          <Button variant="outlined" color="error" onClick={handleDelete}>
            {t('quickReplies.delete')}
          </Button>
        </Stack>
      </Stack>

      <Card>
        <CardContent>
          {content.summary && (
            <Typography variant="subtitle1" color="text.secondary" gutterBottom dir="auto">
              {content.summary}
            </Typography>
          )}
          <Typography variant="body1" sx={{ whiteSpace: 'pre-wrap' }} dir="auto">
            {content.body}
          </Typography>
        </CardContent>
      </Card>

      <ContentFormDialog
        open={dialogOpen}
        content={content}
        onClose={() => setDialogOpen(false)}
        onSubmit={handleSubmit}
        submitting={updateMutation.isPending}
        error={formError}
      />

      <Snackbar
        open={!!snackbarMessage}
        autoHideDuration={5000}
        onClose={() => setSnackbarMessage(null)}
      >
        <Alert severity="error" onClose={() => setSnackbarMessage(null)}>
          {snackbarMessage}
        </Alert>
      </Snackbar>
    </Box>
  )
}
