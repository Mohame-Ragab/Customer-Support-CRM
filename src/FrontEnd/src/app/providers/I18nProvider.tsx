import { useEffect, type ReactNode } from 'react'
import { I18nextProvider } from 'react-i18next'

import { useDirection } from '@/hooks/useDirection'
import i18n from '@/lib/i18n/i18n'

interface I18nProviderProps {
  children: ReactNode
}

/**
 * Initializes react-i18next and keeps `document.documentElement.dir`/`lang`
 * in sync with the active language - the one place that DOM side effect
 * happens (see docs/frontend-architecture.md, "RTL").
 */
export function I18nProvider({ children }: I18nProviderProps) {
  return (
    <I18nextProvider i18n={i18n}>
      <DocumentDirectionSync />
      {children}
    </I18nextProvider>
  )
}

function DocumentDirectionSync() {
  const direction = useDirection()

  useEffect(() => {
    document.documentElement.dir = direction
    document.documentElement.lang = i18n.language
  }, [direction])

  return null
}
