'use client'

import { useMutation } from '@tanstack/react-query'

import { signUp } from '../api/auth'

export const useSignUp = () => {
  return useMutation({
    mutationFn: signUp
  })
}
