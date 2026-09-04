import { useQuery } from '@tanstack/react-query'
import { getPublicBranding } from '../api/brandingApi'

/**
 * Public branding fetch - used app-wide (ThemeProvider, Header logo slot,
 * and the admin form itself) since GET /api/branding/public works whether
 * or not the caller is authenticated.
 */
export function useBranding() {
  return useQuery({
    queryKey: ['branding'],
    queryFn: getPublicBranding,
    staleTime: 5 * 60_000,
  })
}
