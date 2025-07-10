'use client'

import Image from 'next/image'
import { FormProvider, useForm } from 'react-hook-form'
import { z } from 'zod'
import { zodResolver } from '@hookform/resolvers/zod'

import { Select } from '@/shared/ui'

import classes from './PaymentCountrySelect.module.scss'

type OptionA = {
  label: string | React.ReactNode
  value: string
}

const schema = z.object({
  paymentCountry: z.object({
    label: z.string(),
    value: z.string()
  })
})

type FormSchema = z.infer<typeof schema>

export const PaymentCountrySelect = () => {
  const useFormProps = useForm<FormSchema>({
    resolver: zodResolver(schema)
  })

  const countries: OptionA[] = [
    {
      label: 'Брюней',
      value: 'Брюней'
    },
    {
      label: 'Россия',
      value: 'Россия'
    },
    {
      label: 'Казахстан',
      value: 'Казахстан'
    },
    {
      label: 'Беларусь',
      value: 'Беларусь'
    },
    {
      label: 'Молдова',
      value: 'Молдова'
    },
    {
      label: 'Латвия',
      value: 'Латвия'
    },
    {
      label: 'Литва',
      value: 'Литва'
    },
    {
      label: 'Эстония',
      value: 'Эстония'
    },
    {
      label: 'Турция',
      value: 'Турция'
    }
  ]

  return (
    <FormProvider {...useFormProps}>
      <Select
        name='paymentCountry'
        options={countries}
        placeholder={
          <div className={classes.placeholder}>
            <span>Регион оплаты:</span>{' '}
            <div className={classes.placeholderCountry}>
              <Image src='/placeholders/deposit-country.png' width={25} height={19} alt='Брюней' />
              <span>Брюней</span>
            </div>
          </div>
        }
        noOptionsMessage={() => 'Такого региона нет'}
        components={{
          SingleValue: ({ data }) => {
            if (!data) return null

            return (
              <div className={classes.placeholder}>
                <span>Регион оплаты:</span>{' '}
                <div className={classes.placeholderCountry}>
                  <Image
                    src='/placeholders/deposit-country.png'
                    width={25}
                    height={19}
                    alt='Брюней'
                  />
                  <span>{data.value}</span>
                </div>
              </div>
            )
          },
          Option: ({ data, selectOption }) => {
            return (
              <div
                className={classes.option}
                onClick={() => {
                  selectOption(data)
                }}
              >
                <Image
                  src='/placeholders/deposit-country.png'
                  width={25}
                  height={19}
                  alt='Брюней'
                />

                <span>{data.value}</span>
              </div>
            )
          }
        }}
      />
    </FormProvider>
  )
}
