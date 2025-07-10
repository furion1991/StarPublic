import { API } from '@/shared/api'

import type { UserResponse } from './user.types'

export const getMe = async () => {
  const { data } = await API.get<UserResponse>('/users/me')

  return data.result
}

export const getUserById = async (userId: string) => {
  const { data } = await API.get<UserResponse>(`/users/id/${userId}`)

  return data.result
}
