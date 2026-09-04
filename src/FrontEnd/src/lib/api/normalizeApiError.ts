import type { AxiosError } from 'axios'

import type { ApiError, ProblemDetails } from '@/types/api'
import { isValidationProblemDetails } from '@/types/api'

/**
 * Converts any Axios failure (a ProblemDetails response body, a network
 * error, a timeout, ...) into the single `ApiError` shape the rest of the
 * app depends on, so UI code never has to branch on Axios/HTTP specifics.
 */
export function normalizeApiError(error: unknown): ApiError {
  const axiosError = error as AxiosError<ProblemDetails>

  if (!axiosError.isAxiosError) {
    return { status: null, title: 'Unexpected error', detail: String(error) }
  }

  if (!axiosError.response) {
    // Network failure, timeout, CORS rejection, etc. - no response to read.
    return {
      status: null,
      title: axiosError.code === 'ECONNABORTED' ? 'Request timed out' : 'Network error',
      detail: axiosError.message,
    }
  }

  const { status, data } = axiosError.response

  if (data && typeof data === 'object') {
    return {
      status,
      title: data.title ?? 'Request failed',
      detail: data.detail,
      traceId: data.traceId,
      validationErrors: isValidationProblemDetails(data) ? data.errors : undefined,
    }
  }

  return { status, title: 'Request failed' }
}
