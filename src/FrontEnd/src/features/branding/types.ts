export interface Branding {
  primaryColor: string
  secondaryColor: string
  logoDataUrl: string | null
}

export interface UpdateBrandingRequest {
  primaryColor: string
  secondaryColor: string
  logoBase64: string | null
  logoContentType: string | null
}
