import type { TypographyVariantsOptions } from '@mui/material/styles'

/**
 * One font stack that covers both supported languages, with an Arabic-capable
 * fallback (Tahoma/Segoe UI both ship broad Arabic glyph coverage on Windows;
 * system-ui/Roboto cover Latin elsewhere) rather than swapping font families
 * per locale, which would need feature-specific RTL logic (not allowed - see
 * docs/frontend-architecture.md, "RTL").
 */
export const typography: TypographyVariantsOptions = {
  fontFamily: [
    '"Segoe UI"',
    'Tahoma',
    'Roboto',
    'system-ui',
    '-apple-system',
    'Arial',
    'sans-serif',
  ].join(','),
}
