import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { useTranslation } from 'react-i18next'
import {
  Alert,
  Box,
  Button,
  Container,
  MenuItem,
  Snackbar,
  Stack,
  Table,
  TableBody,
  TableCell,
  TableHead,
  TablePagination,
  TableRow,
  TextField,
  Typography,
} from '@mui/material'
import { LoadingState } from '@/components/ui/LoadingState'
import { ErrorState } from '@/components/ui/ErrorState'
import { EmptyState } from '@/components/ui/EmptyState'
import { normalizeApiError } from '@/lib/api/normalizeApiError'
import {
  useContentList,
  useCreateContent,
  useDeleteContent,
} from '@/features/knowledge-base/hooks/useKnowledgeBaseContent'
import {
  ContentFormDialog,
  type ContentFormValues,
} from '@/features/knowledge-base/components/ContentFormDialog'
import { ContentTypeChip } from '@/features/knowledge-base/components/ContentTypeChip'
import { ContentType, type KnowledgeBaseContentListItem } from '@/features/knowledge-base/types'

/**
 * List + inline "create" dialog, mirroring the established F10
 * UsersListPage pattern. Editing an existing item happens on
 * ContentDetailPage (which needs the full Body anyway - the list row
 * intentionally omits it), not via a second dialog here - see Important
 * Decisions in the final report for why this departs from the plan's literal
 * separate create/edit page layout.
 */
export function ContentListPage() {
  const { t } = useTranslation()
  const navigate = useNavigate()

  const [page, setPage] = useState(0)
  const [pageSize, setPageSize] = useState(20)
  const [search, setSearch] = useState('')
  const [typeFilter, setTypeFilter] = useState<ContentType | ''>('')
  const [languageFilter, setLanguageFilter] = useState('')

  const { data, isLoading, isError, error, refetch } = useContentList({
    page: page + 1,
    pageSize,
    search: search || undefined,
    type: typeFilter || undefined,
    language: languageFilter || undefined,
  })

  const createMutation = useCreateContent()
  const [dialogOpen, setDialogOpen] = useState(false)
  const [formError, setFormError] = useState<string | null>(null)
  const deleteMutation = useDeleteContent()
  const [snackbarMessage, setSnackbarMessage] = useState<string | null>(null)

  const handleSubmit = (values: ContentFormValues) => {
    setFormError(null)
    createMutation
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

  const handleDelete = (item: KnowledgeBaseContentListItem) => {
    if (!window.confirm(t('knowledgeBase.confirmDelete', { defaultValue: 'Delete this item?' })))
      return
    deleteMutation.mutate(item.id, {
      onError: (err) => setSnackbarMessage(normalizeApiError(err).detail ?? t('errors.unexpected')),
    })
  }

  return (
    <Container maxWidth="lg" sx={{ py: 4 }}>
      <Stack direction="row" sx={{ justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
        <Typography variant="h4" component="h1">
          {t('knowledgeBase.title')}
        </Typography>
        <Button
          variant="contained"
          onClick={() => {
            setFormError(null)
            setDialogOpen(true)
          }}
        >
          {t('knowledgeBase.create', { defaultValue: 'New content' })}
        </Button>
      </Stack>

      <Stack direction="row" spacing={2} sx={{ mb: 3 }}>
        <TextField
          label={t('knowledgeBase.search.placeholder')}
          value={search}
          onChange={(e) => {
            setSearch(e.target.value)
            setPage(0)
          }}
          size="small"
          sx={{ flex: 1 }}
        />
        <TextField
          select
          label={t('knowledgeBase.form.type', { defaultValue: 'Type' })}
          value={typeFilter}
          onChange={(e) => {
            setTypeFilter(e.target.value === '' ? '' : (Number(e.target.value) as ContentType))
            setPage(0)
          }}
          size="small"
          sx={{ minWidth: 160 }}
        >
          <MenuItem value="">{t('knowledgeBase.allTypes', { defaultValue: 'All types' })}</MenuItem>
          <MenuItem value={ContentType.Faq}>{t('knowledgeBase.type.faq')}</MenuItem>
          <MenuItem value={ContentType.Article}>{t('knowledgeBase.type.article')}</MenuItem>
          <MenuItem value={ContentType.SolutionGuide}>
            {t('knowledgeBase.type.solutionGuide')}
          </MenuItem>
        </TextField>
        <TextField
          select
          label={t('knowledgeBase.form.language', { defaultValue: 'Language' })}
          value={languageFilter}
          onChange={(e) => {
            setLanguageFilter(e.target.value)
            setPage(0)
          }}
          size="small"
          sx={{ minWidth: 140 }}
        >
          <MenuItem value="">
            {t('knowledgeBase.allLanguages', { defaultValue: 'All languages' })}
          </MenuItem>
          <MenuItem value="en">{t('knowledgeBase.language.en')}</MenuItem>
          <MenuItem value="ar">{t('knowledgeBase.language.ar')}</MenuItem>
        </TextField>
      </Stack>

      {isLoading && <LoadingState />}

      {isError && (
        <ErrorState
          title={t('errors.unexpected')}
          message={normalizeApiError(error).detail}
          onRetry={() => refetch()}
        />
      )}

      {!isLoading && !isError && data && data.items.length === 0 && (
        <EmptyState title={t('knowledgeBase.list.empty', { defaultValue: 'No content found.' })} />
      )}

      {!isLoading && !isError && data && data.items.length > 0 && (
        <Box sx={{ overflowX: 'auto' }}>
          <Table>
            <TableHead>
              <TableRow>
                <TableCell>{t('knowledgeBase.form.title', { defaultValue: 'Title' })}</TableCell>
                <TableCell>{t('knowledgeBase.form.type', { defaultValue: 'Type' })}</TableCell>
                <TableCell>
                  {t('knowledgeBase.form.language', { defaultValue: 'Language' })}
                </TableCell>
                <TableCell>{t('knowledgeBase.published')}</TableCell>
                <TableCell align="right">{t('quickReplies.delete')}</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {data.items.map((item) => (
                <TableRow key={item.id} hover>
                  <TableCell
                    dir="auto"
                    onClick={() => navigate(`/knowledge-base/${item.id}`)}
                    sx={{ cursor: 'pointer' }}
                  >
                    {item.title}
                  </TableCell>
                  <TableCell>
                    <ContentTypeChip type={item.type} />
                  </TableCell>
                  <TableCell>
                    {t(`knowledgeBase.language.${item.language}`, { defaultValue: item.language })}
                  </TableCell>
                  <TableCell>
                    {item.isPublished
                      ? t('knowledgeBase.publishedYes', { defaultValue: 'Published' })
                      : t('knowledgeBase.publishedNo', { defaultValue: 'Draft' })}
                  </TableCell>
                  <TableCell align="right">
                    <Button size="small" color="error" onClick={() => handleDelete(item)}>
                      {t('quickReplies.delete')}
                    </Button>
                  </TableCell>
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

      <ContentFormDialog
        open={dialogOpen}
        content={null}
        onClose={() => setDialogOpen(false)}
        onSubmit={handleSubmit}
        submitting={createMutation.isPending}
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
    </Container>
  )
}
