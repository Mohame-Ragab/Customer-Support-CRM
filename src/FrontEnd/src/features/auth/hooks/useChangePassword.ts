import { useMutation } from '@tanstack/react-query'
import { AxiosError } from 'axios'
import { changePassword, type ChangePasswordPayload } from '../api/changePassword'

export interface ChangePasswordError {
  code: string
  message: string
  errors?: Record<string, string[]>
}

export interface UseChangePasswordReturn {
  mutate: (payload: ChangePasswordPayload) => void
  isPending: boolean
  error: ChangePasswordError | null
}

export function useChangePassword(): UseChangePasswordReturn {
  const mutation = useMutation({
    mutationFn: changePassword,
  })

  let error: ChangePasswordError | null = null
  if (mutation.error) {
    const axiosError = mutation.error as AxiosError<ChangePasswordError>
    if (axiosError.response?.data) {
      error = axiosError.response.data
    } else {
      error = {
        code: 'UnexpectedError',
        message: mutation.error.message,
      }
    }
  }

  return {
    mutate: (payload: ChangePasswordPayload) => mutation.mutate(payload),
    isPending: mutation.isPending,
    error,
  }
}
