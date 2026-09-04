import { useEffect, useRef, useState } from 'react'
import { useTranslation } from 'react-i18next'
import { Alert, Box, Button, Paper, Stack, TextField, Typography } from '@mui/material'
import type { HubConnection } from '@microsoft/signalr'
import { HubConnectionState } from '@microsoft/signalr'
import { LoadingState } from '@/components/ui/LoadingState'
import { ErrorState } from '@/components/ui/ErrorState'
import { normalizeApiError } from '@/lib/api/normalizeApiError'
import { QuickReplyPickerButton } from '@/features/quick-replies/components/QuickReplyPickerButton'
import { insertQuickReplyIntoTextarea } from '@/features/quick-replies/hooks/useInsertQuickReply'
import { buildChatConnection } from '../api/chatClient'
import { getChatMessages } from '../api/chatApi'
import { ChatMessageKind, type ChatMessage, type ChatSession } from '../types'

interface AgentChatWindowProps {
  session: ChatSession
}

export function AgentChatWindow({ session }: AgentChatWindowProps) {
  const { t } = useTranslation()
  const connectionRef = useRef<HubConnection | null>(null)

  const [messages, setMessages] = useState<ChatMessage[]>([])
  const [connectionState, setConnectionState] = useState<HubConnectionState>(
    HubConnectionState.Disconnected,
  )
  const [draft, setDraft] = useState('')
  const [error, setError] = useState<string | null>(null)
  const draftFieldRef = useRef<HTMLInputElement | null>(null)

  const handlePickQuickReply = (body: string) => {
    if (draftFieldRef.current) {
      insertQuickReplyIntoTextarea(draftFieldRef.current, body)
    } else {
      setDraft((prev) => (prev ? `${prev} ${body}` : body))
    }
  }

  useEffect(() => {
    let cancelled = false
    let connection: HubConnection | null = null

    async function init() {
      try {
        const history = await getChatMessages(session.id)
        if (cancelled) return
        setMessages(history)

        connection = buildChatConnection()
        connection.on('MessagePosted', (message: ChatMessage) => {
          if (message.chatSessionId !== session.id) return
          setMessages((prev) => (prev.some((m) => m.id === message.id) ? prev : [...prev, message]))
        })
        connection.onreconnected(() => setConnectionState(HubConnectionState.Connected))
        connection.onreconnecting(() => setConnectionState(HubConnectionState.Reconnecting))

        await connection.start()
        if (cancelled) {
          await connection.stop()
          return
        }
        await connection.invoke('JoinSession', session.id)
        await connection.invoke('AssignToMe', session.id)
        connectionRef.current = connection
        setConnectionState(connection.state)
      } catch (err) {
        if (!cancelled) {
          setError(normalizeApiError(err).detail ?? t('chat.disconnected'))
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
  }, [session.id])

  const handleSend = async () => {
    const body = draft.trim()
    const connection = connectionRef.current
    if (!body || !connection || connection.state !== HubConnectionState.Connected) {
      return
    }
    setDraft('')
    try {
      await connection.invoke('SendMessage', session.id, body)
    } catch (err) {
      setError(err instanceof Error ? err.message : t('chat.disconnected'))
    }
  }

  if (error && messages.length === 0) {
    return <ErrorState title={t('errors.unexpected')} message={error} />
  }

  if (connectionState === HubConnectionState.Disconnected && messages.length === 0) {
    return <LoadingState />
  }

  return (
    <Stack spacing={2} sx={{ height: '100%' }}>
      {error && <Alert severity="warning">{error}</Alert>}

      <Paper variant="outlined" sx={{ p: 2, flex: 1, overflowY: 'auto', minHeight: 300 }}>
        <Stack spacing={1}>
          {messages.map((message) => (
            <Box
              key={message.id}
              sx={{
                alignSelf: message.kind === ChatMessageKind.Agent ? 'flex-end' : 'flex-start',
                bgcolor:
                  message.kind === ChatMessageKind.System
                    ? 'action.hover'
                    : message.kind === ChatMessageKind.Agent
                      ? 'primary.main'
                      : 'grey.200',
                color:
                  message.kind === ChatMessageKind.Agent ? 'primary.contrastText' : 'text.primary',
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
        <QuickReplyPickerButton onPick={handlePickQuickReply} />
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
          inputRef={draftFieldRef}
        />
        <Button variant="contained" onClick={() => void handleSend()} disabled={!draft.trim()}>
          {t('chat.send')}
        </Button>
      </Box>
    </Stack>
  )
}
