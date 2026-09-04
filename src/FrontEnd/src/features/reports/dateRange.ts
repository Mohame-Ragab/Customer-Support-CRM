/** Default report window: the last 30 days, as YYYY-MM-DD strings (date-only, matches the backend's DateOnly-bound endpoints). */
export function defaultDateRange(): { from: string; to: string } {
  const to = new Date()
  const from = new Date()
  from.setDate(from.getDate() - 30)
  const format = (d: Date) => d.toISOString().slice(0, 10)
  return { from: format(from), to: format(to) }
}
