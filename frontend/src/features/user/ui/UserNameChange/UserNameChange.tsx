'use client'

import { FormProvider, useForm } from 'react-hook-form'
import { z } from 'zod'
import { zodResolver } from '@hookform/resolvers/zod'
import { useMutation, useQueryClient } from '@tanstack/react-query'

import { TextField } from '@/shared/ui'

import { changeUserInfo } from '../../api/user'
import { useUser } from '@/shared/hooks'

import classes from './UserNameChange.module.scss'

const schema = z.object({
  newUserName: z.string().min(1, { message: 'Поле обязательно' })
})
type FormSchema = z.infer<typeof schema>

type UserNameChangeProps = {
  userName: string
  onUserNameSubmit: () => void
}

export const UserNameChange = ({ userName, onUserNameSubmit }: UserNameChangeProps) => {
  const { user } = useUser()
  const queryClient = useQueryClient()

  const useFormProps = useForm<FormSchema>({
    resolver: zodResolver(schema),
    defaultValues: {
      newUserName: userName
    }
  })
  const {
    handleSubmit,
    formState: { errors }
  } = useFormProps

  const { mutate: changeUserName, isPending: isUserNameChanging } = useMutation({
    mutationFn: changeUserInfo,
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: ['me']
      })
    }
  })

  const onFormSubmit = ({ newUserName }: FormSchema) => {
    if (!user) return

    changeUserName({
      id: user.id,
      userName: newUserName
    })

    onUserNameSubmit()
  }

  return (
    <FormProvider {...useFormProps}>
      <form className={classes.userNameChange} onSubmit={handleSubmit(onFormSubmit)}>
        <TextField
          className={classes.textField}
          name='newUserName'
          placeholder='Введите новый никнейм'
          defaultValue=''
        />

        <button className={classes.changeBtn} type='submit' disabled={isUserNameChanging}>
          Сохранить
        </button>
      </form>
    </FormProvider>
  )
}
