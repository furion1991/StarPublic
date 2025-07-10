'use client'

import { SignalrDataContext } from '@/app/providers'
import { useContext } from 'react'

export const useSignalrData = () => {
  return useContext(SignalrDataContext)
}
