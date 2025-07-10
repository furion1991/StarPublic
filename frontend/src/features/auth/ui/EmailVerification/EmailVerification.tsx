'use client'

import cn from 'classnames'
import { useState } from 'react'

import { Button } from '@/shared/ui'

import { authModalBaseClasses } from '@/entities/auth'
import classes from './EmailVerification.module.scss'

type EmailVerificationProps = {
  isVerificationSending: boolean
  onVerificationSubmit: () => void
}

export const EmailVerification = ({
  isVerificationSending,
  onVerificationSubmit
}: EmailVerificationProps) => {
  const [isVerificationSended, setVerificationSended] = useState(false)

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
            onVerificationSubmit()
          }}
        >
          Отправить подтверждение
        </Button>
      ) : (
        <p>Сообщение на почту отправлено!</p>
      )}
    </div>
  )
}
