import { ErrorBoundary } from '@/components/common/ErrorBoundary'
import { AppRouter } from '@/routes/AppRouter'

import { AppProviders } from './providers/AppProviders'

/**
 * Bootstraps the application: providers, then the router. No business logic
 * lives here - see app/providers/AppProviders.tsx for what each provider does.
 */
export function App() {
  return (
    <ErrorBoundary>
      <AppProviders>
        <AppRouter />
      </AppProviders>
    </ErrorBoundary>
  )
}
