export type SystemSettingValueType = 'String' | 'Integer' | 'Boolean' | 'Decimal' | 'Json'

export interface SystemSetting {
  id: string
  key: string
  value: string
  valueType: SystemSettingValueType
  description?: string | null
  isRequired: boolean
}

export interface UpdateSystemSettingRequest {
  value: string
}
