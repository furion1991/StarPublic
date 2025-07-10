'use client'

import { useQuery } from '@tanstack/react-query'
import { getMe } from '../api/user'

export const useMe = () => {
  return useQuery({
    queryFn: getMe,
    queryKey: ['me']
  })
}
