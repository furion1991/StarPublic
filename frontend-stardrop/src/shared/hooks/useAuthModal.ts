'use client'

import { useContext } from 'react'

import { AuthModalContext } from '@/app/providers'

export const useAuthModal = () => {
  return useContext(AuthModalContext)
}
