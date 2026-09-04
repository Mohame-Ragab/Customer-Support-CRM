import { useEffect, useRef, useState } from 'react'
import { useTranslation } from 'react-i18next'
import { Alert, Box, Button, Paper, Stack, TextField, Typography } from '@mui/material'
import type { HubConnection } from '@microsoft/signalr'
import { HubConnectionState } from '@microsoft/signalr'
import { LoadingState } from '@/components/ui/LoadingState'
import { ErrorState } from '@/components/ui/ErrorState'
import { normalizeApiError } from '@/lib/api/normalizeApiError'
import { buildChatConnection } from '../api/chatClient'
import { getChatMessages, startChatSession } from '../api/chatApi'
import { ChatMessageKind, type ChatMessage, type ChatSession } from '../types'

export function CustomerChatPanel() {
  const { t } = useTranslation()
  const connectionRef = useRef<HubConnection | null>(null)

  const [session, setSession] = useState<ChatSession | null>(null)
  const [messages, setMessages] = useState<ChatMessage[]>([])
  const [connectionState, setConnectionState] = useState<HubConnectionState>(
    HubConnectionState.Disconnected,
  )
  const [draft, setDraft] = useState('')
  const [loadError, setLoadError] = useState<string | null>(null)

  useEffect(() => {
    let cancelled = false
    let connection: HubConnection | null = null

    async function init() {
      try {
        const startedSession = await startChatSession()
        if (cancelled) return
        setSession(startedSession)

        const history = await getChatMessages(startedSession.id)
        if (cancelled) return
        setMessages(history)

        connection = buildChatConnection()
        connection.on('MessagePosted', (message: ChatMessage) => {
          setMessages((prev) => (prev.some((m) => m.id === message.id) ? prev : [...prev, message]))
        })
        connection.onreconnected(() => setConnectionState(HubConnectionState.Connected))
        connection.onreconnecting(() => setConnectionState(HubConnectionState.Reconnecting))

        await connection.start()
        if (cancelled) {
          await connection.stop()
          return
        }
        await connection.invoke('JoinSession', startedSession.id)
        connectionRef.current = connection
        setConnectionState(connection.state)
      } catch (err) {
        if (!cancelled) {
          setLoadError(normalizeApiError(err).detail ?? t('chat.disconnected'))
        }
      }
    }

    void init()

    return () => {
      cancelled = true
      connection?.stop()
      connectionRef.current = null
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [])

  const handleSend = async () => {
    const body = draft.trim()
    const connection = connectionRef.current
    if (!body || !session || !connection || connection.state !== HubConnectionState.Connected) {
      return
    }
    setDraft('')
    try {
      await connection.invoke('SendMessage', session.id, body)
    } catch (err) {
      setLoadError(err instanceof Error ? err.message : t('chat.disconnected'))
    }
  }

  if (loadError && !session) {
    return <ErrorState title={t('errors.unexpected')} message={loadError} />
  }

  if (!session) {
    return <LoadingState />
  }

  return (
    <Stack spacing={2} sx={{ height: '100%' }}>
      {connectionState !== HubConnectionState.Connected && (
        <Alert severity="info">{t('chat.connecting')}</Alert>
      )}
      {loadError && <Alert severity="warning">{loadError}</Alert>}

      <Paper variant="outlined" sx={{ p: 2, flex: 1, overflowY: 'auto', minHeight: 300 }}>
        <Stack spacing={1}>
          {messages.map((message) => (
            <Box
              key={message.id}
              sx={{
                alignSelf: message.kind === ChatMessageKind.Customer ? 'flex-end' : 'flex-start',
                bgcolor:
                  message.kind === ChatMessageKind.System
                    ? 'action.hover'
                    : message.kind === ChatMessageKind.Customer
                      ? 'primary.main'
                      : 'grey.200',
                color:
                  message.kind === ChatMessageKind.Customer
                    ? 'primary.contrastText'
                    : 'text.primary',
                borderRadius: 2,
                px: 1.5,
                py: 1,
                maxWidth: '80%',
              }}
            >
              <Typography variant="body2">{message.body}</Typography>
            </Box>
          ))}
        </Stack>
      </Paper>

      <Box sx={{ display: 'flex', gap: 1 }}>
        <TextField
          fullWidth
          size="small"
          placeholder={t('chat.placeholder')}
          value={draft}
          onChange={(e) => setDraft(e.target.value)}
          onKeyDown={(e) => {
            if (e.key === 'Enter' && !e.shiftKey) {
              e.preventDefault()
              void handleSend()
            }
          }}
        />
        <Button variant="contained" onClick={() => void handleSend()} disabled={!draft.trim()}>
          {t('chat.send')}
        </Button>
      </Box>
    </Stack>
  )
}
