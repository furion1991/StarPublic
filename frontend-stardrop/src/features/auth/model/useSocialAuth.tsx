'use client'

import { useMutation } from '@tanstack/react-query'
import { useSearchParams } from 'next/navigation'
import { useEffect } from 'react'

import { socialAuth } from '../api/auth'

type UseSocialAuthProps = {
  isAuth: boolean
  isAuthInitializing: boolean
  onAuthSuccess: () => void
}

export const useSocialAuth = ({
  isAuth,
  isAuthInitializing,
  onAuthSuccess
}: UseSocialAuthProps) => {
  const searchParams = useSearchParams()

  const { mutate: authFromSocial } = useMutation({
    mutationFn: socialAuth,
    onSuccess: () => {
      onAuthSuccess()
      window.location.hash = ''
    }
  })

  useEffect(() => {
    if (isAuth || isAuthInitializing) return

    const handleTelegramAuth = () => {
      const hash = window.location.hash
      const isTelegramHashExist = hash.startsWith('#tgAuthResult=')

      if (!isTelegramHashExist) return

      const authHashValue = hash.split('=')[1]

      authFromSocial({
        provider: 'telegram',
        data: authHashValue
      })
    }

    const handleVkAuth = () => {
      const state = localStorage.getItem('state')
      const deviceId = searchParams?.get('device_id')
      const codeVerifier = localStorage.getItem('codeVerifier')
      const code = searchParams?.get('code')

      if (!state || !deviceId || !codeVerifier || !code) return

      authFromSocial({
        provider: 'vk',
        vkData: {
          state,
          deviceId,
          codeVerifier,
          code
        }
      })
    }

    handleTelegramAuth()
    handleVkAuth()
  }, [isAuth, isAuthInitializing])
}
