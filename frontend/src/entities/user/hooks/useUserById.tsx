'use client'

import { useQuery } from '@tanstack/react-query'

import { getUserById } from '../api/user'

type UseUserByIdProps = {
  id: string
}

export const useUserById = ({ id }: UseUserByIdProps) => {
  return useQuery({
    queryKey: ['user', id],
    queryFn: () => getUserById(id)
  })
}
