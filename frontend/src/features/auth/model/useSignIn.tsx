'use client'

import { useMutation } from '@tanstack/react-query'
import { AxiosError, AxiosResponse } from 'axios'

import { useAuth } from '@/shared/hooks'
import { signIn } from '../api/auth'

type SignInError = {
  error: string
  statusCode: number
}

type SignInResponse = {
  message: string
}

type AuthProps = {
  email: string
  password: string
}

type UseSignInProps = {
  onSuccess: () => void
}

export const useSignIn = ({ onSuccess }: UseSignInProps) => {
  const { setAuth } = useAuth()

  return useMutation<AxiosResponse<SignInResponse>, AxiosError<SignInError>, AuthProps>({
    mutationFn: signIn,
    onSuccess: () => {
      setAuth(true)
      onSuccess()
    }
  })
}
