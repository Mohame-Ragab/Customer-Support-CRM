import { createTheme, type Theme } from '@mui/material/styles'

import type { Direction } from '@/types/common'

import { applyBrandOverrides, darkPalette, lightPalette, type BrandColors } from './palette'
import { typography } from './typography'

export type ThemeMode = 'light' | 'dark'

/**
 * The one place a MUI theme is constructed. Direction and color mode are
 * parameters (not hardcoded) because both are runtime, user-driven concerns
 * (selected language / a future theme toggle) - see app/providers/ThemeProvider.tsx.
 * `brand` is optional (platform/custom-branding) - omitting it keeps behavior
 * identical to before that story, so existing call sites do not break.
 */
export function createAppTheme(mode: ThemeMode, direction: Direction, brand?: BrandColors): Theme {
  const basePalette = mode === 'light' ? lightPalette : darkPalette

  return createTheme({
    direction,
    palette: applyBrandOverrides(basePalette, brand),
    typography,
    shape: { borderRadius: 8 },
    spacing: 8,
  })
}
