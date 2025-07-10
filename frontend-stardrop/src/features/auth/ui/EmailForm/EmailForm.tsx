'use client'

import { FormProvider, useForm } from 'react-hook-form'
import { z } from 'zod'
import { zodResolver } from '@hookform/resolvers/zod'
import { useMutation } from '@tanstack/react-query'

import { Button, TextField } from '@/shared/ui'

import { checkIsEmailExist } from '../../api/auth'

import { authModalBaseClasses } from '@/entities/auth'

const schema = z.object({
  email: z
    .string()
    .min(1, { message: 'Поле обязательно' })
    .email({ message: 'Некорректный e-mail' })
})

type FormSchema = z.infer<typeof schema>
type EmailData = { email: string; isEmailExist?: boolean }

type EmailFormProps = {
  onEmailSubmit: ({ email, isEmailExist }: EmailData) => void
}

export const EmailForm = ({ onEmailSubmit }: EmailFormProps) => {
  const checkEmail = useMutation({
    mutationFn: checkIsEmailExist
  })

  const useFormProps = useForm<FormSchema>({
    resolver: zodResolver(schema)
  })
  const {
    handleSubmit,
    formState: { errors }
  } = useFormProps

  const emailError = errors?.email?.message

  const onFormSubmit = async ({ email }: FormSchema) => {
    const isEmailExist = await checkEmail.mutateAsync({ email })
    onEmailSubmit({ email, isEmailExist })
  }

  return (
    <FormProvider {...useFormProps}>
      <form style={{ position: 'relative' }} onSubmit={handleSubmit(onFormSubmit)}>
        <h4 className={authModalBaseClasses.heading}>Войдите или создайте аккаунт</h4>

        <div className={authModalBaseClasses.inputRow}>
          <TextField name='email' placeholder='Введите e-mail' />
        </div>

        <Button
          loading={checkEmail.isPending}
          className={authModalBaseClasses.submitBtn}
          type='submit'
          fullWidth
        >
          Продолжить ›
        </Button>

        {emailError ? <div className={authModalBaseClasses.formError}>{emailError}</div> : null}
      </form>
    </FormProvider>
  )
}
