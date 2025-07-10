'use client'

import { FormProvider, useForm } from 'react-hook-form'
import { z } from 'zod'
import { zodResolver } from '@hookform/resolvers/zod'

import { Button, TextField } from '@/shared/ui'

import { authModalBaseClasses } from '@/entities/auth'

const schema = z.object({
  password: z.string().min(8, { message: 'Пароль должен иметь не менее 8 символов' })
})

type FormSchema = z.infer<typeof schema>

type CreatePasswordFormProps = {
  onPasswordSubmit: (password: string) => void
}

export const CreatePasswordForm = ({ onPasswordSubmit }: CreatePasswordFormProps) => {
  const useFormProps = useForm<FormSchema>({
    resolver: zodResolver(schema)
  })
  const {
    handleSubmit,
    formState: { errors }
  } = useFormProps

  const passwordError = errors?.password?.message

  const onFormSubmit = ({ password }: FormSchema) => {
    onPasswordSubmit(password)
  }

  return (
    <FormProvider {...useFormProps}>
      <form style={{ position: 'relative' }} onSubmit={handleSubmit(onFormSubmit)}>
        <h4 className={authModalBaseClasses.heading}>Придумайте пароль</h4>

        <div className={authModalBaseClasses.inputRow}>
          <TextField type='password' name='password' placeholder='Не менее 8 символов' />
        </div>

        <Button className={authModalBaseClasses.submitBtn} type='submit' fullWidth>
          Создать аккаунт
        </Button>

        {passwordError ? (
          <div className={authModalBaseClasses.formError}>{passwordError}</div>
        ) : null}
      </form>
    </FormProvider>
  )
}
