'use client'

import { useMutation, useQueryClient } from '@tanstack/react-query'
import { createContext, useState, useEffect } from 'react'
import { usePathname, useRouter, useSearchParams } from 'next/navigation'

import { signOut, useSocialAuth } from '@/features/auth'

type HandleProtectedRoutesRedirectsProps = {
  isAuth: boolean
  pathname: string
}

type AuthContextProps = {
  isAuth: boolean
  isAuthInitializing: boolean
  setAuthInitializing: (value: boolean) => void
  setAuth: (value: boolean) => void
  logout: () => void
}

export const AuthContext = createContext({} as AuthContextProps)

export const AuthProvider = ({ children }: { children: React.ReactNode }) => {
  const queryClient = useQueryClient()
  const pathname = usePathname()
  const router = useRouter()
  const searchParams = useSearchParams()

  const [isAuth, setAuth] = useState(false)
  const [isAuthInitializing, setAuthInitializing] = useState(true)

  useSocialAuth({
    isAuth,
    isAuthInitializing,
    onAuthSuccess: () => {
      setAuth(true)
    }
  })

  const { mutate: logout } = useMutation({
    mutationFn: signOut,
    onSuccess: onLogoutSuccess
  })

  useEffect(() => {
    if (!pathname || isAuthInitializing) return

    const handleProtectedRoutesRedirects = ({
      isAuth,
      pathname
    }: HandleProtectedRoutesRedirectsProps) => {
      const protectedRoutes = ['/profile', '/deposit']
      const isProtectedRoute = protectedRoutes.includes(pathname)

      if (isAuth && pathname === '/auth-required') {
        const redirectUrl = searchParams?.get('redirect_url')
        router.replace(redirectUrl || '/')
        return
      }

      if (!isAuth && isProtectedRoute && pathname !== '/auth-required') {
        router.push(`/auth-required?redirect_url=${pathname}`)
        return
      }
    }

    handleProtectedRoutesRedirects({ isAuth, pathname })
  }, [isAuth, pathname, isAuthInitializing])

  function onLogoutSuccess() {
    if (pathname === '/profile' || pathname === '/deposit') {
      router.push('/')
    }

    queryClient.removeQueries({
      queryKey: ['me']
    })
  }

  return (
    <AuthContext.Provider
      value={{
        isAuth,
        isAuthInitializing,
        setAuthInitializing,
        setAuth,
        logout
      }}
    >
      {children}
    </AuthContext.Provider>
  )
}
