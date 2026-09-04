import { createContext } from 'react'

import type { AuthState } from '@/types/auth'

export interface AuthContextValue extends AuthState {
  /**
   * Adopts an already-issued access token (decodes it, stores it, marks the
   * session authenticated). Does NOT call the backend itself - the future
   * login feature is responsible for obtaining the token and passing it
   * here. Kept this way so AuthProvider never needs to know how a token was
   * obtained (password login, SSO, ...).
   */
  login: (accessToken: string) => void
  logout: () => void
}

export const AuthContext = createContext<AuthContextValue | undefined>(undefined)
