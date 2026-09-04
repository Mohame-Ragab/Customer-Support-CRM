import type { AuthUser } from '@/types/auth'

/**
 * Role-check helpers for navigation/UX decisions only (e.g. hiding a nav
 * item). They are NOT a security boundary: any authenticated user can alter
 * client-side state, so every real authorization decision must be enforced
 * by the backend (see src/BackEnd - [Authorize(Roles = ...)] against
 * Domain.Constants.Roles). Never rely on these to protect sensitive data or
 * actions - only to avoid showing UI the backend would reject anyway.
 */

export function hasRole(user: AuthUser | null, role: string): boolean {
  return user?.roles.includes(role) ?? false
}

export function hasAnyRole(user: AuthUser | null, roles: string[]): boolean {
  return roles.some((role) => hasRole(user, role))
}

// TODO(story: security-admin/effective-permissions): this delegates to the
// Admin role check until a "/me" endpoint returns the caller's effective
// permission set (security-admin/manage-role-permissions introduced the
// backend catalog/assignment API, but not yet a way for the frontend to know
// which permissions the current user's role(s) actually grant).
export function hasPermission(user: AuthUser | null, _permission: string): boolean {
  return hasRole(user, 'Admin')
}
