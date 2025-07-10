'use client'

import { useEffect, useState } from 'react'

type UseCountdownProps = {
  countdownSeconds: number
}

export const useCountdown = ({ countdownSeconds }: UseCountdownProps) => {
  // only bugged in dev mode because of React StrictMode, in prod works fine
  const [resendCountdown, setResendCountdown] = useState(countdownSeconds)
  const [countdownInterval, setCountdownInterval] = useState<NodeJS.Timeout | null>(null)
  const [isResendAllowed, setResendAllowed] = useState(false)

  useEffect(() => {
    startCountdown()
  }, [])

  useEffect(() => {
    if (resendCountdown === 0) {
      setResendAllowed(true)
      resetCountdown()
    }
  }, [resendCountdown])

  const startCountdown = () => {
    setResendAllowed(false)

    setCountdownInterval(
      setInterval(() => {
        setResendCountdown((prev) => prev - 1)
      }, 1000)
    )
  }

  const resetCountdown = () => {
    setResendCountdown(countdownSeconds)

    if (countdownInterval) {
      clearInterval(countdownInterval)
    }
  }

  return {
    isResendAllowed,
    resendCountdown,
    startCountdown
  }
}
