'use client'

import Image from 'next/image'
import { useRef } from 'react'

import { useAuth, useAuthModal } from '@/shared/hooks'
import { Button, PriceWithCurrency, SparksAnimated } from '@/shared/ui'

import { useColorsAnimation } from '../../hooks/useColorsAnimations'

import classes from './ContractItemsFillPanel.module.scss'

type ContractItemsFillPanelProps = {
  itemValueStart?: number
  itemValueEnd?: number
  isContractInProcess: boolean
  isItemValueLoading: boolean
  itemsInContractNumber: number
  glowing: boolean
  isSparksActive: boolean
  onContractSubmit: () => void
}

export const ContractItemsFillPanel = ({
  itemValueStart,
  itemValueEnd,
  isContractInProcess,
  isItemValueLoading,
  itemsInContractNumber,
  glowing,
  isSparksActive,
  onContractSubmit
}: ContractItemsFillPanelProps) => {
  const { isAuth } = useAuth()
  const { openAuthModal } = useAuthModal()

  const progressFillRef = useRef<HTMLDivElement>(null)

  useColorsAnimation({
    elementRef: progressFillRef,
    enabled: glowing
  })

  return (
    <div className={classes.contractItemsFillPanel}>
      <div className={classes.box}>
        <p className={classes.mainText}>
          {!isAuth ? 'Авторизуйтесь, чтобы добавить предметы' : null}

          {isAuth && (!itemValueStart || !itemValueEnd) && !isItemValueLoading
            ? 'Добавьте предметы для контракта'
            : null}

          {isAuth && itemValueStart && itemValueEnd && !isItemValueLoading ? (
            <>
              Получите предмет стоимостью от <PriceWithCurrency>{itemValueStart}</PriceWithCurrency>{' '}
              до <PriceWithCurrency>{itemValueEnd}</PriceWithCurrency>
            </>
          ) : null}

          {isItemValueLoading ? ' Загрузка стоимости предметов...' : null}
        </p>

        <div className={classes.progressBar}>
          <div
            ref={progressFillRef}
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

      <SparksAnimated active={isSparksActive} />

      <Image
        className={classes.bgItems}
        src='/img/contracts/contract-bg-items.png'
        width={1260}
        height={599}
        quality={100}
        alt='предметы'
      />

      <Image
        className={classes.bgLight}
        src='/img/contracts/contract-bg-light.png'
        width={1482}
        height={575}
        alt='Фон'
      />

      <div className={classes.bgLight} />

      <Image
        className={classes.boxBg}
        src='/img/contracts/contract-box-bg.svg'
        width={1147}
        height={135}
        alt='Фон'
      />

      {isAuth ? (
        <Button
          loading={isContractInProcess}
          className={classes.btn}
          borderRadius='medium'
          onClick={onContractSubmit}
        >
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
