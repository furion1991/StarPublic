'use client'

import { FormProvider, useForm } from 'react-hook-form'
import { z } from 'zod'
import { zodResolver } from '@hookform/resolvers/zod'
import { useState } from 'react'
import Image from 'next/image'

import { Button, TextField } from '@/shared/ui'

import classes from './PaymentPromocodeForm.module.scss'

const schema = z.object({
  promocode: z.string().min(1, { message: 'Поле обязательно' })
})

type FormSchema = z.infer<typeof schema>

type PaymentPromocodeFormProps = {
  onPromocodeSubmit: (promocode: string) => void
}

export const PaymentPromocodeForm = ({ onPromocodeSubmit }: PaymentPromocodeFormProps) => {
  const useFormProps = useForm<FormSchema>({
    resolver: zodResolver(schema)
  })
  const { handleSubmit } = useFormProps

  const [isPromocodeEntered, setPromocodeEntered] = useState(false)

  const onFormSubmit = ({ promocode }: FormSchema) => {
    onPromocodeSubmit(promocode)
    setPromocodeEntered(true)
  }

  return (
    <FormProvider {...useFormProps}>
      <div className={classes.paymentPromocodeForm}>
        <form className={classes.form} onSubmit={handleSubmit(onFormSubmit)}>
          <TextField
            className={classes.textField}
            name='promocode'
            placeholder='Промокод (если есть)'
          />

          <Button type='submit'>Применить</Button>
        </form>

        {isPromocodeEntered ? (
          <div className={classes.promocode}>
            <p>Бонус на пополнение</p>
            <p>+50% к пополнению</p>

            <div className={classes.promocodeImg}>
              <Image src='/img/wallet.png' width={127} height={138} alt='Кошелек' />
            </div>
          </div>
        ) : null}
      </div>
    </FormProvider>
  )
}
