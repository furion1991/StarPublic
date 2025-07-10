'use client'

import { FormProvider, useForm } from 'react-hook-form'
import { z } from 'zod'
import { zodResolver } from '@hookform/resolvers/zod'
import Image from 'next/image'
import Link from 'next/link'
import { useRouter } from 'next/navigation'

import { Button, Switch, TextField } from '@/shared/ui'

import classes from './PaymentForm.module.scss'

const schema = z.object({
  email: z.string().email().min(1, { message: 'Поле обязательно' }),
  amount: z.string().min(1, { message: 'Поле обязательно' }),
  agreement: z.literal<boolean>(true, { message: 'Примите условия соглашения' })
})

type FormSchema = z.infer<typeof schema>

export const PaymentForm = () => {
  const router = useRouter()

  const useFormProps = useForm<FormSchema>({
    resolver: zodResolver(schema)
  })
  const { handleSubmit } = useFormProps

  const onFormSubmit = ({ email, amount, agreement }: FormSchema) => {
    console.log({ email, amount })
    router.push('/deposit-success')
  }

  return (
    <FormProvider {...useFormProps}>
      <form className={classes.paymentForm} onSubmit={handleSubmit(onFormSubmit)}>
        <div className={classes.formBox}>
          <TextField className={classes.textField} type='email' name='email' placeholder='E-mail' />

          <TextField
            className={classes.textField}
            type='number'
            name='amount'
            placeholder='Сумма пополнения'
            endAdornment={
              <div className={classes.endAdornment}>
                <Image src='/icons/coins.svg' width={20} height={20} alt='Монеты' />
              </div>
            }
          />

          <label className={classes.agreement}>
            <Switch className={classes.agreementSwitch} name='agreement' />{' '}
            <p>
              Я принимаю условия <Link href=''>пользовательского соглашения</Link>
            </p>
          </label>
        </div>

        <Button
          className={classes.paymentBtn}
          type='submit'
          fullWidth
          color='purple'
          borderRadius='medium'
        >
          Перейти к оплате ›
        </Button>
      </form>
    </FormProvider>
  )
}
