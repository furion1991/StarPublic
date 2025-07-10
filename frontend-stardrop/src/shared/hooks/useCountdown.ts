'use client'

import { useEffect, useState } from 'react'

type UseCountdownProps = {
  countdownSeconds: number
}

export const useCountdown = ({ countdownSeconds }: UseCountdownProps) => {
  const [countdown, setCountdown] = useState(countdownSeconds)
  const [countdownInterval, setCountdownInterval] = useState<NodeJS.Timeout | null>(null)
  const [isResendAllowed, setResendAllowed] = useState(false)

  useEffect(() => {
    if (countdown === 0) {
      setResendAllowed(true)
      resetCountdown()
    }
  }, [countdown])

  const startCountdown = () => {
    setResendAllowed(false)

    setCountdownInterval(
      setInterval(() => {
        setCountdown((prev) => prev - 1)
      }, 1000)
    )
  }

  const resetCountdown = () => {
    setCountdown(countdownSeconds)

    if (countdownInterval) {
      clearInterval(countdownInterval)
    }
  }

  return {
    isResendAllowed,
    countdown,
    startCountdown
  }
}
