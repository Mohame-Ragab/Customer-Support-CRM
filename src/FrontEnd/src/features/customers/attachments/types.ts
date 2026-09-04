export interface CustomerAttachment {
  id: string
  customerId: string
  fileName: string
  contentType: string
  sizeBytes: number
  uploadedBy: string | null
  uploadedAt: string
}
