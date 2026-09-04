import { useState } from 'react'
import { useTranslation } from 'react-i18next'
import { Alert, Box, Button, List, ListItem, ListItemText, TextField } from '@mui/material'
import { LoadingState } from '@/components/ui/LoadingState'
import { ErrorState } from '@/components/ui/ErrorState'
import { EmptyState } from '@/components/ui/EmptyState'
import { normalizeApiError } from '@/lib/api/normalizeApiError'
import { useAddCustomerNote, useCustomerNotes } from './useCustomerNotes'

interface CustomerNotesPanelProps {
  customerId: string
}

/** Append-only staff notes on a customer (F01 customers/manage-customer-notes; minimal frontend added by F04's ticket-workspace customer panel). */
export function CustomerNotesPanel({ customerId }: CustomerNotesPanelProps) {
  const { t } = useTranslation()
  const { data: notes, isLoading, isError, error, refetch } = useCustomerNotes(customerId)
  const mutation = useAddCustomerNote(customerId)
  const [content, setContent] = useState('')
  const [formError, setFormError] = useState<string | null>(null)

  const handleAdd = () => {
    const trimmed = content.trim()
    if (!trimmed) return
    setFormError(null)
    mutation.mutate(trimmed, {
      onSuccess: () => setContent(''),
      onError: (err) => setFormError(normalizeApiError(err).detail ?? t('errors.unexpected')),
    })
  }

  return (
    <Box>
      {isLoading && <LoadingState />}
      {isError && (
        <ErrorState message={normalizeApiError(error).detail} onRetry={() => refetch()} />
      )}
      {!isLoading && !isError && (notes?.length ?? 0) === 0 && (
        <EmptyState title={t('customers.notes.empty', { defaultValue: 'No notes yet' })} />
      )}
      {!isLoading && !isError && (notes?.length ?? 0) > 0 && (
        <List>
          {(notes ?? []).map((note) => (
            <ListItem key={note.id} divider alignItems="flex-start">
              <ListItemText
                primary={<span dir="auto">{note.content}</span>}
                secondary={new Date(note.createdAt).toLocaleString()}
              />
            </ListItem>
          ))}
        </List>
      )}

      <Box sx={{ mt: 2 }}>
        {formError && (
          <Alert severity="error" sx={{ mb: 1 }}>
            {formError}
          </Alert>
        )}
        <TextField
          fullWidth
          multiline
          minRows={2}
          placeholder={t('customers.notes.placeholder', { defaultValue: 'Add a note…' })}
          value={content}
          onChange={(e) => setContent(e.target.value)}
          disabled={mutation.isPending}
          slotProps={{ htmlInput: { dir: 'auto' } }}
        />
        <Box sx={{ mt: 1, textAlign: 'right' }}>
          <Button
            variant="contained"
            size="small"
            onClick={handleAdd}
            disabled={mutation.isPending || !content.trim()}
          >
            {t('customers.notes.add', { defaultValue: 'Add note' })}
          </Button>
        </Box>
      </Box>
    </Box>
  )
}
