import { useMutation } from '@tanstack/react-query'
import { useNavigate } from 'react-router-dom'
import { AxiosError } from 'axios'
import { useAuth } from './useAuth'
import { postLogin, type LoginRequestBody } from '../api/login'

export interface LoginErrorResponse {
  code: string
  message: string
}

export interface UseLoginMutationReturn {
  mutate: (credentials: LoginRequestBody) => void
  isPending: boolean
  error: LoginErrorResponse | null
}

export function useLoginMutation(): UseLoginMutationReturn {
  const navigate = useNavigate()
  const { login } = useAuth()

  const mutation = useMutation({
    mutationFn: postLogin,
    onSuccess: (data) => {
      // The backend also sets the refresh token as an HttpOnly cookie on
      // this response (see AuthController.Login) - nothing to store for it
      // client-side; see lib/auth/tokenStorage.ts.
      login(data.accessToken)
      navigate('/', { replace: true })
    },
  })

  let error: LoginErrorResponse | null = null
  if (mutation.error) {
    const axiosError = mutation.error as AxiosError<LoginErrorResponse>
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
    mutate: (credentials: LoginRequestBody) => mutation.mutate(credentials),
    isPending: mutation.isPending,
    error,
  }
}
