import { apiClient } from '@/lib/api/apiClient'
import type { SystemSetting, UpdateSystemSettingRequest } from '../types'

export async function listSystemSettings(): Promise<SystemSetting[]> {
  const response = await apiClient.get('/api/system-configuration')
  return response.data
}

export async function updateSystemSetting(key: string, body: UpdateSystemSettingRequest): Promise<SystemSetting> {
  const response = await apiClient.put(`/api/system-configuration/${encodeURIComponent(key)}`, body)
  return response.data
}
