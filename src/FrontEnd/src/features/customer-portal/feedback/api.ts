import { apiClient } from '@/lib/api/apiClient'

export interface SubmitFeedbackInput {
  rating: number
  comment?: string
}

export interface SubmitFeedbackResponse {
  feedbackId: string
  submittedAt: string
}

export async function submitFeedback(
  ticketId: string,
  input: SubmitFeedbackInput,
): Promise<SubmitFeedbackResponse> {
  const response = await apiClient.post(`/api/customer-portal/tickets/${ticketId}/feedback`, input)
  return response.data
}
