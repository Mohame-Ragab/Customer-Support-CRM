/**
 * Centralized, validated access to build-time environment variables.
 *
 * Vite only exposes variables prefixed `VITE_` to the browser bundle (see
 * .env.example) - never add a backend secret here. Reading `import.meta.env`
 * directly from feature code is avoided so a missing/misconfigured variable
 * fails loudly, once, at startup, instead of producing a confusing runtime
 * error deep inside an API call. Known variables are typed in vite-env.d.ts.
 */

interface RequiredEnv {
  apiBaseUrl: string
}

function requireNonEmpty(name: string, value: string | undefined): string {
  if (!value || value.trim().length === 0) {
    throw new Error(
      `Missing required environment variable "${name}". ` +
        'Copy .env.example to .env.local and set it before starting the app.',
    )
  }
  return value
}

function loadEnv(): RequiredEnv {
  return {
    apiBaseUrl: requireNonEmpty('VITE_API_BASE_URL', import.meta.env.VITE_API_BASE_URL),
  }
}

export const env = loadEnv()
