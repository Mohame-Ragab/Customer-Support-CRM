import type { JwtClaims } from '@/types/auth'

/**
 * Reads the (unverified) payload out of a JWT for UI display purposes only -
 * e.g. showing the signed-in user's name or deciding which nav items to show.
 *
 * This intentionally does NOT verify the token's signature; that is neither
 * possible nor meaningful client-side (verification requires the backend's
 * signing key, which must never reach the browser - see docs, "Security
 * boundaries"). Every claim read here is advisory. The backend independently
 * validates the token's signature and expiry on every request and is the
 * only real security boundary (see src/BackEnd's JwtBearer configuration).
 */
export function decodeJwtPayload(token: string): JwtClaims | null {
  const parts = token.split('.')
  if (parts.length !== 3 || !parts[1]) {
    return null
  }

  try {
    const base64Url = parts[1]
    const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/')
    const json = decodeURIComponent(
      atob(base64)
        .split('')
        .map((c) => '%' + c.charCodeAt(0).toString(16).padStart(2, '0'))
        .join(''),
    )
    return JSON.parse(json) as JwtClaims
  } catch {
    return null
  }
}

export function isTokenExpired(claims: JwtClaims): boolean {
  if (typeof claims.exp !== 'number') {
    // No expiry claim: treat as expired rather than trusting it indefinitely.
    return true
  }
  return claims.exp * 1000 <= Date.now()
}

export function rolesFromClaims(claims: JwtClaims): string[] {
  if (!claims.role) return []
  return Array.isArray(claims.role) ? claims.role : [claims.role]
}
