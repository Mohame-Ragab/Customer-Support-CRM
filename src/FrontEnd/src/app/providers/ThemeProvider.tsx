import { useMemo, type ReactNode } from 'react'
import createCache from '@emotion/cache'
import { CacheProvider } from '@emotion/react'
import CssBaseline from '@mui/material/CssBaseline'
import { ThemeProvider as MuiThemeProvider } from '@mui/material/styles'
import { prefixer } from 'stylis'
import rtlPlugin from 'stylis-plugin-rtl'

import { useDirection } from '@/hooks/useDirection'
import { useBranding } from '@/features/branding/hooks/useBranding'
import { createAppTheme } from '@/styles/theme/createAppTheme'

interface ThemeProviderProps {
  children: ReactNode
}

// One Emotion cache per direction, created once (not per render) - MUI's
// documented approach for RTL: https://mui.com/material-ui/customization/right-to-left/
const ltrCache = createCache({ key: 'mui' })
const rtlCache = createCache({ key: 'mui-rtl', stylisPlugins: [prefixer, rtlPlugin] })

/**
 * Design-system entry point: builds the MUI theme for the active direction
 * and provides it, plus CssBaseline for a consistent baseline. Dark mode is
 * prepared in styles/theme/palette.ts but not switchable yet - light is the
 * default until a real theme-toggle requirement exists (avoids
 * over-engineering an unused setting).
 */
export function ThemeProvider({ children }: ThemeProviderProps) {
  const direction = useDirection()
  // While the branding query is loading (or on a fresh install with no
  // reachable branding row), `data` is undefined - createAppTheme falls back
  // to the default palette unchanged (platform/custom-branding). No need to
  // block rendering; the palette swap on load is acceptable, same as i18n boot.
  const { data: brand } = useBranding()
  const theme = useMemo(
    () => createAppTheme('light', direction, brand ?? undefined),
    [direction, brand],
  )

  return (
    <CacheProvider value={direction === 'rtl' ? rtlCache : ltrCache}>
      <MuiThemeProvider theme={theme}>
        <CssBaseline />
        {children}
      </MuiThemeProvider>
    </CacheProvider>
  )
}
