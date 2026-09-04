# Auth feature

The one feature folder allowed real implementation at this stage (per the
architecture task's scope), because routing/authorization infrastructure
depends on it existing. Contains:

- `context/AuthContext.ts` — the context object/type.
- `context/AuthProvider.tsx` — owns authentication _state_ (split from the
  context definition so the component-only file supports React Fast Refresh).
- `hooks/useAuth.ts` — consumes the context.
- `authorization.ts` — `hasRole`/`hasAnyRole` UX helpers (not a security boundary).

**Not implemented:** login/register API calls, password reset, MFA, user
management. `AuthProvider.login()` adopts an already-issued access token; it
does not know how one was obtained. The first real auth feature work is
wiring an actual login form + API call that obtains a token and calls it —
see `docs/frontend-architecture.md`, "Next step".
