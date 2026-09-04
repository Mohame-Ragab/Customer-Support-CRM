/**
 * Authentication/authorization shapes. No login/register implementation lives
 * here - only the state shape the future auth feature will populate once the
 * backend exposes a token endpoint (see src/BackEnd, JWT authentication).
 */

/**
 * Claims read out of the JWT access token's payload (see lib/auth/jwt.ts).
 * Matches standard ASP.NET Core Identity + JWT claim names; all optional
 * because a token is never trusted to contain more than it says it does.
 */
export interface JwtClaims {
  sub?: string
  name?: string
  email?: string
  role?: string | string[]
  exp?: number
  [claim: string]: unknown
}

export interface AuthUser {
  id: string
  userName?: string
  email?: string
  roles: string[]
}

export type AuthStatus = 'idle' | 'authenticated' | 'unauthenticated'

export interface AuthState {
  status: AuthStatus
  user: AuthUser | null
}

export interface RegisterCustomerRequest {
  email: string
  password: string
}

export interface RegisterCustomerResponse {
  userId: string
  email: string
}
