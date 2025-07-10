import { FormProvider, useForm } from 'react-hook-form'
import { z } from 'zod'
import { zodResolver } from '@hookform/resolvers/zod'

import { Button, TextField } from '@/shared/ui'

import { authModalBaseClasses } from '@/entities/auth'
import classes from './EnterPasswordForm.module.scss'

const schema = z.object({
  password: z.string().min(8, { message: 'Пароль должен иметь не менее 8 символов' })
})

type FormSchema = z.infer<typeof schema>

type EnterPasswordFormProps = {
  error?: string
  loading: boolean
  onPasswordSubmit: (password: string) => void
  onPasswordReset: () => void
}

export const EnterPasswordForm = ({
  error,
  loading,
  onPasswordSubmit,
  onPasswordReset
}: EnterPasswordFormProps) => {
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
        <h4 className={authModalBaseClasses.heading}>Введите свой пароль</h4>

        <div className={authModalBaseClasses.inputRow}>
          <TextField type='password' name='password' placeholder='Не менее 8 символов' />
        </div>

        <Button
          loading={loading}
          className={authModalBaseClasses.submitBtn}
          type='submit'
          fullWidth
        >
          Войти в аккаунт
        </Button>

        <button type='button' className={classes.forgotPassword} onClick={onPasswordReset}>
          Забыли свой пароль?
        </button>

        {passwordError || error ? (
          <div className={authModalBaseClasses.formError}>{passwordError || error}</div>
        ) : null}
      </form>
    </FormProvider>
  )
}
