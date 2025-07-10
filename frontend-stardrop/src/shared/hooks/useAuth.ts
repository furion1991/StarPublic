'use client'

import { useContext } from 'react'

import { AuthContext } from '@/app/providers'

export const useAuth = () => {
  return useContext(AuthContext)
}
