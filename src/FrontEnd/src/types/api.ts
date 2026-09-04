/**
 * Shapes describing the backend's HTTP contract (see
 * src/BackEnd/docs/architecture.md, "Global exception handling" and
 * "API response strategy"). Kept generic and cross-feature - no per-entity
 * response types belong here.
 */

/** RFC 7807 ProblemDetails, as returned by CustomerSupportCRM.API's GlobalExceptionHandler. */
export interface ProblemDetails {
  status?: number
  title?: string
  detail?: string
  instance?: string
  /** Extension member added by the backend (HttpContext.TraceIdentifier). */
  traceId?: string
}

/** ASP.NET Core's ValidationProblemDetails shape: ProblemDetails + per-property error messages. */
export interface ValidationProblemDetails extends ProblemDetails {
  errors: Record<string, string[]>
}

export function isValidationProblemDetails(
  value: ProblemDetails,
): value is ValidationProblemDetails {
  return 'errors' in value && typeof (value as ValidationProblemDetails).errors === 'object'
}

/**
 * Normalized error shape produced by lib/api/apiClient.ts for every failed
 * request, regardless of whether the backend returned a ProblemDetails body,
 * a network failure occurred, or something unexpected happened. UI code
 * should depend on this type, not on Axios/ProblemDetails directly.
 */
export interface ApiError {
  status: number | null
  title: string
  detail?: string
  traceId?: string
  /** Present only for 400 validation failures; property name -> messages. */
  validationErrors?: Record<string, string[]>
}
