import type { ReactNode } from 'react'

import { AuthProvider } from '@/features/auth/context/AuthProvider'

import { I18nProvider } from './I18nProvider'
import { QueryProvider } from './QueryProvider'
import { ThemeProvider } from './ThemeProvider'

interface AppProvidersProps {
  children: ReactNode
}

/**
 * Single composition point for every app-wide provider. Order matters:
 * I18nProvider first (ThemeProvider reads the active language for RTL/LTR),
 * then QueryProvider - ThemeProvider now calls useBranding() (a TanStack
 * Query hook, platform/custom-branding) to apply brand color overrides, so it
 * must be nested *below* QueryProvider, not above it - then ThemeProvider,
 * then AuthProvider (global auth state - see features/auth).
 */
export function AppProviders({ children }: AppProvidersProps) {
  return (
    <I18nProvider>
      <QueryProvider>
        <ThemeProvider>
          <AuthProvider>{children}</AuthProvider>
        </ThemeProvider>
      </QueryProvider>
    </I18nProvider>
  )
}
