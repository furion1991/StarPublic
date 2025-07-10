'use client'

import { useContext } from 'react'
import { SignalrContext } from '@/app/providers'

export const useSignalr = () => {
  return useContext(SignalrContext)
}
