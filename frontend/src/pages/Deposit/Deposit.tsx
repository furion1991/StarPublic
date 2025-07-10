'use client'

import Image from 'next/image'
import { useState } from 'react'

import { PaymentOptionsList } from '@/entities/payments'
import { PaymentCountrySelect, PaymentForm, PaymentPromocodeForm } from '@/features/payments'

import classes from './Deposit.module.scss'

export const DepositPage = () => {
  const [promocode, setPromocode] = useState('')

  return (
    <div className={classes.depositPage}>
      <div className={classes.wrapper}>
        <div className={classes.container}>
          <div className={classes.heading}>
            <Image src='/icons/wallet-gradient.svg' width={24} height={24} alt='Кошелек' />
            <h1>Пополнение баланса</h1>
          </div>

          <div className={classes.content}>
            <div className={classes.contentLeft}>
              <PaymentCountrySelect />

              <div className={classes.paymentOptions}>
                <PaymentOptionsList />
              </div>

              <p className={classes.minAmount}>
                Минимальная сумма пополнения <span>10 ₽</span>
              </p>
            </div>

            <div className={classes.contentRight}>
              <div className={classes.selectedPaymentOption}>
                <div className={classes.selectedPaymentOptionName}>
                  <p>Выбранный метод оплаты:</p>
                  <p>Cards</p>
                </div>

                <Image
                  src='/placeholders/deposit-umoney.png'
                  width={99}
                  height={21}
                  alt='Ю-Money'
                />
              </div>

              <div className={classes.promocode}>
                <PaymentPromocodeForm onPromocodeSubmit={setPromocode} />
              </div>

              <div className={classes.payment}>
                <PaymentForm />
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  )
}
