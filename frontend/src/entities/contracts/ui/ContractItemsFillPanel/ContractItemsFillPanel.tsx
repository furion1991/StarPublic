'use client'

import Image from 'next/image'

import { useAuth, useAuthModal } from '@/shared/hooks'
import { Button, PriceWithCurrency } from '@/shared/ui'

import classes from './ContractItemsFillPanel.module.scss'

type ContractItemsFillPanelProps = {
  itemsInContractNumber: number
  onContractSubmit: () => void
}

export const ContractItemsFillPanel = ({
  itemsInContractNumber,
  onContractSubmit
}: ContractItemsFillPanelProps) => {
  const { isAuth } = useAuth()
  const { openAuthModal } = useAuthModal()

  return (
    <div className={classes.contractItemsFillPanel}>
      <div className={classes.box}>
        <p className={classes.mainText}>
          {!isAuth ? 'Авторизуйтесь, чтобы добавить предметы' : null}

          {isAuth && !itemsInContractNumber ? 'Добавьте предметы для контракта' : null}

          {isAuth && itemsInContractNumber ? (
            <>
              Получите предмет стоимостью от <PriceWithCurrency>500.7</PriceWithCurrency> до{' '}
              <PriceWithCurrency>7920.8</PriceWithCurrency>
            </>
          ) : null}
        </p>

        <div className={classes.progressBar}>
          <div
            className={classes.progressFill}
            style={{ width: `${itemsInContractNumber * 10}%` }}
          />
        </div>

        <div className={classes.ticks}>
          {Array.from({ length: 21 }).map((_, idx) => {
            if (idx === 0)
              return (
                <div className={classes.tick} key={idx}>
                  <div className={classes.tickLine} />
                  <p className={classes.tickLabel}>0</p>
                </div>
              )

            return (
              <div className={classes.tick} key={idx}>
                <div className={classes.tickLine} />
                {idx % 2 === 0 ? <p className={classes.tickLabel}>{idx / 2}</p> : null}
              </div>
            )
          })}
        </div>
      </div>

      <Image
        className={classes.boxBg}
        src='/img/contract-box-bg.svg'
        width={1147}
        height={135}
        alt='Фон'
      />

      {isAuth ? (
        <Button className={classes.btn} borderRadius='medium' onClick={onContractSubmit}>
          Создать контракт
        </Button>
      ) : (
        <Button className={classes.btn} borderRadius='medium' onClick={openAuthModal}>
          Войти
        </Button>
      )}
    </div>
  )
}
