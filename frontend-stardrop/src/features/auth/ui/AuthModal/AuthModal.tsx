'use client'

import { useEffect, useState } from 'react'
import { useMutation } from '@tanstack/react-query'

import { AuthModalBase } from '@/entities/auth'
import { EmailForm } from '../EmailForm/EmailForm'
import { EnterPasswordForm } from '../EnterPasswordForm/EnterPasswordForm'
import { SocialNetworksAuth } from '../SocialNetworksAuth/SocialNetworksAuth'
import { CreatePasswordForm } from '../CreatePasswordForm/CreatePasswordForm'
import { PasswordResetForm } from '../PasswordResetForm/PasswordResetForm'
import { PasswordResetSuccess } from '../PasswordResetSuccess/PasswordResetSuccess'
import { EmailVerification } from '../EmailVerification/EmailVerification'
import { EmailVerificationSuccess } from '../EmailVerificationSuccess/EmailVerificationSuccess'

import { useSignUp } from '../../model/useSignUp'
import { useSignIn } from '../../model/useSignIn'
import { resetPassword } from '../../api/auth'

type Step =
  | 'email'
  | 'password'
  | 'password-reset'
  | 'password-reset-success'
  | 'email-verification'
  | 'email-verification-success'

type EmailData = { email: string; isEmailExist?: boolean }

type AuthModalProps = {
  open: boolean
  verifiedEmail?: string
  onClose: () => void
}

export const AuthModal = ({ open, verifiedEmail, onClose }: AuthModalProps) => {
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [isEmailExist, setEmailExist] = useState<boolean>(false)
  const [step, setStep] = useState<Step>('email')

  const signUp = useSignUp()
  const signIn = useSignIn({
    onSuccess: () => {
      onClose()
      setStep('email')
    }
  })

  const sendResetPasswordMail = useMutation({
    mutationFn: resetPassword,
    onSuccess: () => {
      setEmail(email)
      setStep('password-reset-success')
    }
  })

  useEffect(() => {
    if (verifiedEmail) {
      setEmail(verifiedEmail)
      setEmailExist(true)
      setStep('password')
    }
  }, [verifiedEmail])

  const signInError =
    signIn.error && signIn.error.response?.data.error === 'Invalid Password'
      ? 'Неправильный пароль'
      : undefined

  const setEmailData = ({ email, isEmailExist }: EmailData) => {
    setEmail(email)
    setEmailExist(Boolean(isEmailExist))
    setStep('password')
  }

  const toPrevStep = () => {
    switch (step) {
      case 'email':
        return

      case 'password':
        setStep('email')
        return

      case 'email-verification':
        setStep('password')
        return

      case 'password-reset':
        setStep('password')
        return

      case 'password-reset-success':
        setStep('password')
        return
    }
  }

  return (
    <AuthModalBase
      open={open}
      SocialAuthSlot={<SocialNetworksAuth />}
      stepBackAvailable={step !== 'email' && step !== 'email-verification-success'}
      onStepBack={toPrevStep}
      onClose={onClose}
    >
      {step === 'email' ? <EmailForm onEmailSubmit={setEmailData} /> : null}

      {/* {step === 'code' ? <CodeVerificationForm onCodeSubmit={toPasswordStep} /> : null} */}

      {step === 'password' && isEmailExist ? (
        <EnterPasswordForm
          error={signInError}
          loading={signIn.isPending}
          onPasswordSubmit={(password) => {
            signIn.mutate({ email, password })
          }}
          onPasswordReset={() => {
            setStep('password-reset')
          }}
        />
      ) : null}

      {step === 'password' && !isEmailExist ? (
        <CreatePasswordForm
          onPasswordSubmit={(password) => {
            setPassword(password)
            setStep('email-verification')
          }}
        />
      ) : null}

      {step === 'email-verification' ? (
        <EmailVerification
          isVerificationSending={signUp.isPending}
          email={email}
          onVerificationSend={() => {
            signUp.mutate({ email, password })
          }}
        />
      ) : null}

      {step === 'email-verification-success' ? (
        <EmailVerificationSuccess
          onNextStep={() => {
            setStep('password')
          }}
        />
      ) : null}

      {step === 'password-reset' ? (
        <PasswordResetForm
          loading={sendResetPasswordMail.isPending}
          onPasswordReset={(email) => {
            sendResetPasswordMail.mutate(email)
          }}
        />
      ) : null}

      {step === 'password-reset-success' ? (
        <PasswordResetSuccess
          onStepBack={() => {
            setStep('password')
          }}
        />
      ) : null}
    </AuthModalBase>
  )
}
