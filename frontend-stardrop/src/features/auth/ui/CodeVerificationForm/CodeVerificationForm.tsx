'use client'

import { FormProvider, useForm } from 'react-hook-form'
import { z } from 'zod'
import { zodResolver } from '@hookform/resolvers/zod'

import { authModalBaseClasses } from '@/entities/auth'
import { Button, CodeField } from '@/shared/ui'
import { useCountdown } from '@/shared/hooks'

import classes from './CodeVerificationForm.module.scss'

const schema = z.object({
  code: z
    .string({ message: 'Введите код подтверждения' })
    .min(6, { message: 'Введите код подтверждения полностью' })
})

type FormSchema = z.infer<typeof schema>

type CodeVerificationFormProps = {
  onCodeSubmit: (code: string) => void
}

export const CodeVerificationForm = ({ onCodeSubmit }: CodeVerificationFormProps) => {
  const COUNTDOWN_IN_SECONDS = 30

  const useFormProps = useForm<FormSchema>({
    resolver: zodResolver(schema)
  })
  const {
    handleSubmit,
    formState: { errors }
  } = useFormProps

  // only bugged in dev mode because of React StrictMode, in prod works fine
  const {
    isResendAllowed,
    countdown: resendCountdown,
    startCountdown
  } = useCountdown({
    countdownSeconds: COUNTDOWN_IN_SECONDS
  })

  const codeError = errors?.code?.message

  const resendCode = () => {
    startCountdown()
  }

  const onFormSubmit = ({ code }: FormSchema) => {
    onCodeSubmit(code)
  }

  return (
    <FormProvider {...useFormProps}>
      <form style={{ position: 'relative' }} onSubmit={handleSubmit(onFormSubmit)}>
        <h4 className={authModalBaseClasses.heading}>Введите код</h4>

        <div className={authModalBaseClasses.inputRow}>
          <CodeField name='code' />
        </div>

        <Button className={authModalBaseClasses.submitBtn} type='submit' fullWidth>
          Продолжить
        </Button>

        <div className={classes.resendCodeRow}>
          {!isResendAllowed ? (
            <p className={classes.resendCode}>
              Отправить код повторно через 0:
              {resendCountdown < 10 ? `0${resendCountdown}` : resendCountdown}
            </p>
          ) : (
            <button type='button' className={classes.resendCodeBtn} onClick={resendCode}>
              Отправить код повторно
            </button>
          )}
        </div>

        {codeError ? <div className={authModalBaseClasses.formError}>{codeError}</div> : null}
      </form>
    </FormProvider>
  )
}
