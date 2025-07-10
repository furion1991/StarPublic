import { API } from '@/shared/api'

import type { ChangeUserInfoProps } from './user.types'

export const changeUserInfo = ({ id, userName, email, phone }: ChangeUserInfoProps) => {
  return API.put(`/users/update/${id}`, {
    userName,
    email,
    phone
  })
}
