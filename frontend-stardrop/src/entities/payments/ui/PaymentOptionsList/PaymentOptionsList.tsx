'use client'

import Image from 'next/image'
import { useState } from 'react'
import cn from 'classnames'

import classes from './PaymentOptionsList.module.scss'

export const PaymentOptionsList = () => {
  const [selectedPayment, setSelectedPayment] = useState(0)

  const paymentOptions = Array.from({ length: 18 }).fill(null)

  return (
    <ul className={classes.paymentOptionsList}>
      {paymentOptions.map((_, idx) => {
        return (
          <li
            key={idx}
            className={cn(classes.paymentOption, {
              [classes.selected]: selectedPayment === idx
            })}
            onClick={() => {
              setSelectedPayment(idx)
            }}
          >
            <button type='button'>
              <Image src='/placeholders/deposit-umoney.png' width={99} height={21} alt='ЮMoney' />
            </button>
          </li>
        )
      })}
    </ul>
  )
}
