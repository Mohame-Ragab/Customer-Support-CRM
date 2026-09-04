import { Fragment } from 'react'
import { useTranslation } from 'react-i18next'
import { List, ListItem, ListItemText, Typography } from '@mui/material'
import { LoadingState } from '@/components/ui/LoadingState'
import { ErrorState } from '@/components/ui/ErrorState'
import { EmptyState } from '@/components/ui/EmptyState'
import { normalizeApiError } from '@/lib/api/normalizeApiError'
import type { KnowledgeBaseSearchResult } from '../types'

interface KnowledgeBaseSearchResultsProps {
  query: string
  items: KnowledgeBaseSearchResult[] | undefined
  isLoading: boolean
  isError: boolean
  error: unknown
  onSelect?: (item: KnowledgeBaseSearchResult) => void
}

/** Wraps every whole-word occurrence of any query token in <mark>. */
function highlight(text: string, query: string) {
  const tokens = query
    .trim()
    .split(/\s+/)
    .filter((t) => t.length >= 2)
  if (tokens.length === 0) return text

  const pattern = new RegExp(
    `(${tokens.map((t) => t.replace(/[.*+?^${}()|[\]\\]/g, '\\$&')).join('|')})`,
    'gi',
  )
  const parts = text.split(pattern)

  return parts.map((part, i) =>
    tokens.some((t) => part.toLowerCase() === t.toLowerCase()) ? (
      <mark key={i}>{part}</mark>
    ) : (
      <Fragment key={i}>{part}</Fragment>
    ),
  )
}

export function KnowledgeBaseSearchResults({
  query,
  items,
  isLoading,
  isError,
  error,
  onSelect,
}: KnowledgeBaseSearchResultsProps) {
  const { t } = useTranslation()

  if (isLoading) {
    return <LoadingState />
  }

  if (isError) {
    return (
      <ErrorState
        title={t('knowledgeBase.search.error')}
        message={normalizeApiError(error).detail}
      />
    )
  }

  if (!items || items.length === 0) {
    return <EmptyState title={t('knowledgeBase.search.empty')} />
  }

  return (
    <List>
      {items.map((item) => (
        <ListItem
          key={item.id}
          divider
          onClick={() => onSelect?.(item)}
          sx={onSelect ? { cursor: 'pointer' } : undefined}
        >
          <ListItemText
            primary={<span dir="auto">{highlight(item.title, query)}</span>}
            secondary={
              <Typography component="span" variant="body2" color="text.secondary" dir="auto">
                {highlight(item.excerpt, query)}
              </Typography>
            }
          />
        </ListItem>
      ))}
    </List>
  )
}
