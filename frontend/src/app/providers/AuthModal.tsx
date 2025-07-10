'use client'

import { createContext, useEffect, useState } from 'react'
import { useSearchParams } from 'next/navigation'
import { useMutation } from '@tanstack/react-query'

import { AuthModal } from '@/features/auth'
import { confirmEmail } from '@/features/auth/api/auth'

type AuthModalContextProps = {
  isOpen: boolean
  openAuthModal: () => void
  closeModal: () => void
}

export const AuthModalContext = createContext({} as AuthModalContextProps)

export const AuthModalProvider = ({ children }: { children: React.ReactNode }) => {
  const searchParams = useSearchParams()

  const [isAuthModalOpen, setAuthModalOpen] = useState(false)
  const [email, setEmail] = useState('')

  const verifyEmail = useMutation({
    mutationFn: confirmEmail,
    onSuccess: (_, { email }) => {
      setAuthModalOpen(true)
      setEmail(email)
    }
  })

  useEffect(() => {
    const email = searchParams?.get('email')
    const token = searchParams?.get('token')

    if (email && token) {
      verifyEmail.mutate({
        email,
        token
      })
    }
  }, [])

  return (
    <AuthModalContext.Provider
      value={{
        isOpen: isAuthModalOpen,
        openAuthModal: () => {
          setAuthModalOpen(true)
        },
        closeModal: () => {
          setAuthModalOpen(false)
        }
      }}
    >
      {children}

      <AuthModal
        open={isAuthModalOpen}
        verifiedEmail={email}
        onClose={() => {
          setAuthModalOpen(false)
        }}
      />
    </AuthModalContext.Provider>
  )
}
