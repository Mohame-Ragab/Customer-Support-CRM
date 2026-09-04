import { useTranslation } from 'react-i18next'

import type { Direction } from '@/types/common'

const RTL_LANGUAGES = new Set(['ar'])

/**
 * Single source of truth for "is the current language RTL?". Used by
 * ThemeProvider (MUI direction) and I18nProvider (document.dir/lang) so the
 * two never disagree. No feature should compute direction on its own.
 */
export function useDirection(): Direction {
  const { i18n } = useTranslation()
  return RTL_LANGUAGES.has(i18n.language) ? 'rtl' : 'ltr'
}
