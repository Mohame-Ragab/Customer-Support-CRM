import { apiClient } from '@/lib/api/apiClient'
import type { Branding, UpdateBrandingRequest } from '../types'

export async function getPublicBranding(): Promise<Branding> {
  const response = await apiClient.get('/api/branding/public')
  return response.data
}

export async function getBranding(): Promise<Branding> {
  const response = await apiClient.get('/api/branding')
  return response.data
}

export async function updateBranding(body: UpdateBrandingRequest): Promise<Branding> {
  const response = await apiClient.put('/api/branding', body)
  return response.data
}
