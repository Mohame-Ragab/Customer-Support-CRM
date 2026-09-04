import { apiClient } from '@/lib/api/apiClient'
import type { CustomerAttachment } from './types'

export async function listCustomerAttachments(customerId: string): Promise<CustomerAttachment[]> {
  const response = await apiClient.get(`/api/customers/${customerId}/attachments`)
  return response.data
}

export async function uploadCustomerAttachment(
  customerId: string,
  file: File,
): Promise<CustomerAttachment> {
  const formData = new FormData()
  formData.append('customerId', customerId)
  formData.append('file', file)

  const response = await apiClient.post(`/api/customers/${customerId}/attachments`, formData, {
    headers: { 'Content-Type': 'multipart/form-data' },
  })
  return response.data
}

/**
 * Downloads via apiClient (not a plain <a href>) so the Authorization header
 * is attached, then triggers a browser save using a short-lived object URL.
 */
export async function downloadCustomerAttachment(
  customerId: string,
  attachmentId: string,
  fileName: string,
): Promise<void> {
  const response = await apiClient.get(
    `/api/customers/${customerId}/attachments/${attachmentId}/download`,
    {
      responseType: 'blob',
    },
  )

  const url = URL.createObjectURL(response.data as Blob)
  const link = document.createElement('a')
  link.href = url
  link.download = fileName
  document.body.appendChild(link)
  link.click()
  link.remove()
  URL.revokeObjectURL(url)
}
