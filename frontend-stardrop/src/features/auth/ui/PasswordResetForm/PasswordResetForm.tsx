'use client'

import { FormProvider, useForm } from 'react-hook-form'
import { z } from 'zod'
import { zodResolver } from '@hookform/resolvers/zod'

import { Button, TextField } from '@/shared/ui'

import { authModalBaseClasses } from '@/entities/auth'

const schema = z.object({
  email: z
    .string()
    .min(1, { message: 'Поле обязательно' })
    .email({ message: 'Некорректный e-mail' })
})

type FormSchema = z.infer<typeof schema>

type PasswordResetFormProps = {
  loading: boolean
  onPasswordReset: (email: string) => void
}

export const PasswordResetForm = ({ loading, onPasswordReset }: PasswordResetFormProps) => {
  const useFormProps = useForm<FormSchema>({
    resolver: zodResolver(schema)
  })
  const {
    handleSubmit,
    formState: { errors }
  } = useFormProps

  const emailError = errors?.email?.message

  const onFormSubmit = ({ email }: FormSchema) => {
    onPasswordReset(email)
  }

  return (
    <FormProvider {...useFormProps}>
      <form style={{ position: 'relative' }} onSubmit={handleSubmit(onFormSubmit)}>
        <h4 className={authModalBaseClasses.heading}>Сброс пароля аккаунта</h4>

        <div className={authModalBaseClasses.inputRow}>
          <TextField name='email' placeholder='Введите e-mail' />
        </div>

        <Button
          loading={loading}
          className={authModalBaseClasses.submitBtn}
          type='submit'
          fullWidth
        >
          Отправить запрос
        </Button>

        {emailError ? <div className={authModalBaseClasses.formError}>{emailError}</div> : null}
      </form>
    </FormProvider>
  )
}
