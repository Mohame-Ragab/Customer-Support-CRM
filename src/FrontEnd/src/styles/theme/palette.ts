import type { PaletteOptions } from '@mui/material/styles'

/**
 * Centralized color foundation. Business screens should pull from
 * `theme.palette`, never hardcode a hex color inline - that is the whole
 * point of a design-system layer.
 */
export const lightPalette: PaletteOptions = {
  mode: 'light',
  primary: { main: '#1565c0' },
  secondary: { main: '#546e7a' },
  background: { default: '#f5f6f8', paper: '#ffffff' },
}

export const darkPalette: PaletteOptions = {
  mode: 'dark',
  primary: { main: '#90caf9' },
  secondary: { main: '#b0bec5' },
  background: { default: '#121212', paper: '#1e1e1e' },
}

/** Colors an Administrator can override via platform/custom-branding. */
export interface BrandColors {
  primaryColor: string
  secondaryColor: string
}

/**
 * Returns a shallow-cloned palette with `primary.main`/`secondary.main`
 * replaced by the tenant's brand colors when provided (platform/custom-branding).
 * `lightPalette`/`darkPalette` above stay the untouched defaults - this never
 * mutates them, only the copy handed back.
 */
export function applyBrandOverrides(base: PaletteOptions, brand?: BrandColors): PaletteOptions {
  if (!brand?.primaryColor || !brand?.secondaryColor) {
    return base
  }

  return {
    ...base,
    primary: { ...base.primary, main: brand.primaryColor },
    secondary: { ...base.secondary, main: brand.secondaryColor },
  }
}
