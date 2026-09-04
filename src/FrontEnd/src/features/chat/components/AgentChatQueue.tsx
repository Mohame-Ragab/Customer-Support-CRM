import { useEffect, useState } from 'react'
import { useTranslation } from 'react-i18next'
import { Box, List, ListItemButton, ListItemText, Stack, Typography } from '@mui/material'
import { LoadingState } from '@/components/ui/LoadingState'
import { ErrorState } from '@/components/ui/ErrorState'
import { EmptyState } from '@/components/ui/EmptyState'
import { normalizeApiError } from '@/lib/api/normalizeApiError'
import { getOpenChatSessions } from '../api/chatApi'
import type { ChatSession } from '../types'
import { AgentChatWindow } from './AgentChatWindow'

export function AgentChatQueue() {
  const { t } = useTranslation()
  const [sessions, setSessions] = useState<ChatSession[] | null>(null)
  const [error, setError] = useState<string | null>(null)
  const [selected, setSelected] = useState<ChatSession | null>(null)

  useEffect(() => {
    let cancelled = false
    getOpenChatSessions()
      .then((data) => {
        if (!cancelled) setSessions(data)
      })
      .catch((err) => {
        if (!cancelled) setError(normalizeApiError(err).detail ?? t('errors.unexpected'))
      })
    return () => {
      cancelled = true
    }
  }, [t])

  if (error) {
    return <ErrorState title={t('errors.unexpected')} message={error} />
  }

  if (!sessions) {
    return <LoadingState />
  }

  return (
    <Stack direction={{ xs: 'column', md: 'row' }} spacing={2} sx={{ height: '100%' }}>
      <Box sx={{ minWidth: 260 }}>
        {sessions.length === 0 ? (
          <EmptyState title={t('chat.queueEmpty')} />
        ) : (
          <List>
            {sessions.map((session) => (
              <ListItemButton
                key={session.id}
                selected={selected?.id === session.id}
                onClick={() => setSelected(session)}
              >
                <ListItemText
                  primary={session.ticketId}
                  secondary={new Date(session.startedAtUtc).toLocaleString()}
                />
              </ListItemButton>
            ))}
          </List>
        )}
      </Box>

      <Box sx={{ flex: 1 }}>
        {selected ? (
          <AgentChatWindow key={selected.id} session={selected} />
        ) : (
          <Typography color="text.secondary">
            {t('chat.assignToMe', { defaultValue: 'Select a session to join' })}
          </Typography>
        )}
      </Box>
    </Stack>
  )
}
