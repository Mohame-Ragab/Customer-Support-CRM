import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { useTranslation } from 'react-i18next'
import { Box, Container, List, ListItemButton, ListItemText, Typography } from '@mui/material'
import { LoadingState } from '@/components/ui/LoadingState'
import { ErrorState } from '@/components/ui/ErrorState'
import { EmptyState } from '@/components/ui/EmptyState'
import { normalizeApiError } from '@/lib/api/normalizeApiError'
import {
  KnowledgeBaseSearchBox,
  KnowledgeBaseSearchResults,
  useKnowledgeBaseSearch,
  usePublishedArticles,
} from '@/features/knowledge-base'

/**
 * Reuses F06's public search endpoint/components as-is (KnowledgeBaseSearchBox/
 * Results, useKnowledgeBaseSearch) - this page only adds the "browse without a
 * search term" list, which F06 explicitly left for this story to build.
 */
export function FaqsPage() {
  const { t } = useTranslation()
  const navigate = useNavigate()
  const [query, setQuery] = useState('')
  const [submittedQuery, setSubmittedQuery] = useState('')

  const searchResult = useKnowledgeBaseSearch(submittedQuery)
  const browseResult = usePublishedArticles({ page: 1, pageSize: 20 })

  const isSearching = submittedQuery.trim().length >= 2

  return (
    <Container maxWidth="md" sx={{ py: 4 }}>
      <Typography variant="h4" component="h1" gutterBottom>
        {t('knowledgeBase.title')}
      </Typography>

      <Box sx={{ mb: 3 }}>
        <KnowledgeBaseSearchBox value={query} onChange={setQuery} onSearch={setSubmittedQuery} />
      </Box>

      {isSearching ? (
        <KnowledgeBaseSearchResults
          query={submittedQuery}
          items={searchResult.data?.items}
          isLoading={searchResult.isLoading}
          isError={searchResult.isError}
          error={searchResult.error}
          onSelect={(item) => navigate(`/portal/faqs/${item.id}`)}
        />
      ) : (
        <>
          {browseResult.isLoading && <LoadingState />}
          {browseResult.isError && (
            <ErrorState
              title={t('errors.unexpected')}
              message={normalizeApiError(browseResult.error).detail}
              onRetry={() => browseResult.refetch()}
            />
          )}
          {!browseResult.isLoading &&
            !browseResult.isError &&
            (browseResult.data?.items.length ?? 0) === 0 && (
              <EmptyState title={t('knowledgeBase.list.empty')} />
            )}
          {!browseResult.isLoading &&
            !browseResult.isError &&
            (browseResult.data?.items.length ?? 0) > 0 && (
              <List>
                {(browseResult.data?.items ?? []).map((item) => (
                  <ListItemButton
                    key={item.id}
                    onClick={() => navigate(`/portal/faqs/${item.id}`)}
                    divider
                  >
                    <ListItemText
                      primary={<span dir="auto">{item.title}</span>}
                      secondary={item.summary}
                    />
                  </ListItemButton>
                ))}
              </List>
            )}
        </>
      )}
    </Container>
  )
}
