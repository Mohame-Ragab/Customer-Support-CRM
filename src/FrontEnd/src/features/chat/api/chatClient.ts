import { HubConnectionBuilder, LogLevel, type HubConnection } from '@microsoft/signalr'
import { env } from '@/config/env'
import { getAccessToken } from '@/lib/auth/tokenStorage'

/**
 * One SignalR connection per mounted chat panel/window - not a shared
 * singleton, since a customer and an agent surface never coexist in the same
 * tab and each caller manages its own connection lifecycle (start/stop tied
 * to component mount).
 */
export function buildChatConnection(): HubConnection {
  const hubUrl = `${env.apiBaseUrl.replace(/\/$/, '')}/hubs/chat`

  return new HubConnectionBuilder()
    .withUrl(hubUrl, { accessTokenFactory: () => getAccessToken() ?? '' })
    .withAutomaticReconnect()
    .configureLogging(LogLevel.Warning)
    .build()
}
