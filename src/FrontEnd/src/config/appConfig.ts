/**
 * Static, non-secret application configuration that isn't environment-specific
 * (contrast with env.ts, which reads .env values). Kept as plain constants
 * rather than an environment variable because these values are the same in
 * every deployment.
 */
export const appConfig = {
  appName: 'Customer Support CRM',
  apiTimeoutMs: 15_000,
  defaultLanguage: 'en',
  supportedLanguages: ['en', 'ar'] as const,
} as const

export type SupportedLanguage = (typeof appConfig.supportedLanguages)[number]
