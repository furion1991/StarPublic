'use client'

import { useMutation } from '@tanstack/react-query'

import { resendEmailConfirm } from '../api/auth'

type UseEmailVerificationResend = {
  onSuccess: () => void
}

export const useEmailVerificationResend = ({ onSuccess }: UseEmailVerificationResend) => {
  return useMutation({
    mutationFn: resendEmailConfirm,
    onSuccess
  })
}
