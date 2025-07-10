import { API } from '@/shared/api'

import type { AuthProps, SocialAuthProps, VerifyEmailProps } from './api.types'

export const checkIsEmailExist = async ({ email }: { email: string }) => {
  const { data } = await API.get<boolean>(`/exists/${email}`)

  return data
}

export const signUp = async ({ email, password }: AuthProps) => {
  return API.post('/auth/register/email', {
    email,
    password,
    role: 'User'
  })
}

export const signIn = ({ email, password }: AuthProps) => {
  return API.post('/auth/login', {
    email,
    password
  })
}

export const signOut = () => {
  return API.post('/auth/logout')
}

export const socialAuth = ({ provider, data, vkData }: SocialAuthProps) => {
  return API.post('/auth/social', {
    provider,
    data,
    vkData
  })
}

export const resetPassword = (email: string) => {
  return API.post(`/auth/reset-password?email=${email}`)
}

export const resendEmailConfirm = (email: string) => {
  return API.get(`/resend-email/${email}`)
}

export const confirmEmail = ({ email, token }: VerifyEmailProps) => {
  return API.get('/confirm-email', {
    params: {
      email,
      token
    }
  })
}
