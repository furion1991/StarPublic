'use client'

import cn from 'classnames'
import { useState } from 'react'

import { Button } from '@/shared/ui'

import { useCountdown } from '@/shared/hooks'
import { useEmailVerificationResend } from '../../model/useEmailVerificationResend'

import { authModalBaseClasses } from '@/entities/auth'
import classes from './EmailVerification.module.scss'

type EmailVerificationProps = {
  email: string
  isVerificationSending: boolean
  onVerificationSend: () => void
}

export const EmailVerification = ({
  email,
  isVerificationSending,
  onVerificationSend
}: EmailVerificationProps) => {
  const COUNTDOWN_IN_SECONDS = 10

  const [isVerificationSended, setVerificationSended] = useState(false)
  const {
    isResendAllowed,
    countdown: resendCountdown,
    startCountdown
  } = useCountdown({
    countdownSeconds: COUNTDOWN_IN_SECONDS
  })
  const resendEmailVeirification = useEmailVerificationResend({
    onSuccess: () => {
      startCountdown()
    }
  })

  return (
    <div className={classes.emailVerification}>
      <h4 className={authModalBaseClasses.heading}>
        Подтвердите почту <br /> для входа в аккаунт
      </h4>

      {!isVerificationSended ? (
        <Button
          type='button'
          loading={isVerificationSending}
          className={cn(authModalBaseClasses.submitBtn, classes.btn)}
          onClick={() => {
            setVerificationSended(true)
            onVerificationSend()
            startCountdown()
          }}
        >
          Отправить подтверждение
        </Button>
      ) : (
        <div className={classes.messageSendedContainer}>
          <p>Сообщение на почту отправлено!</p>

          {!isResendAllowed ? (
            <p className={classes.resendCode}>
              Отправить сообщение повторно можно через 0:
              {resendCountdown < 10 ? `0${resendCountdown}` : resendCountdown}
            </p>
          ) : (
            <Button
              type='button'
              loading={resendEmailVeirification.isPending}
              className={cn(authModalBaseClasses.submitBtn)}
              onClick={() => {
                resendEmailVeirification.mutate(email)
              }}
            >
              Отправить код повторно
            </Button>
          )}
        </div>
      )}
    </div>
  )
}
