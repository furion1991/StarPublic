'use client'

import { useContext } from 'react'

import { UserContext } from '@/app/providers'

export const useUser = () => {
  return useContext(UserContext)
}
