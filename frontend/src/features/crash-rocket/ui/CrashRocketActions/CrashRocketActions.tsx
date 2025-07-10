'use client'

import Image from 'next/image'
import { FormProvider, useForm } from 'react-hook-form'
import { useState } from 'react'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'

import { Button, PriceWithCurrency, Switch } from '@/shared/ui'

import classes from './CrashRocketActions.module.scss'

const schema = z.object({
  autoStop: z.boolean()
})

type FormSchema = z.infer<typeof schema>

export const CrashRocketActions = () => {
  const BET_STEP = 100
  const MULTIPLIER_STEP = 0.5

  const [bet, setBet] = useState(0)
  const [multiplier, setMultiplier] = useState(0)

  const useFormProps = useForm<FormSchema>({
    resolver: zodResolver(schema)
  })

  const handleBetDown = () => {
    if (bet === 0) return

    if (bet - BET_STEP <= 0) {
      setBet(0)
      return
    }

    setBet(bet - BET_STEP)
  }

  const handleBetUp = () => {
    setBet(bet + BET_STEP)
  }

  const handleMultiplierDown = () => {
    if (multiplier === 0) return

    if (multiplier - MULTIPLIER_STEP <= 0) {
      setMultiplier(0)
      return
    }

    setMultiplier(multiplier - MULTIPLIER_STEP)
  }

  const handleMultiplierUp = () => {
    setMultiplier(multiplier + MULTIPLIER_STEP)
  }

  return (
    <div className={classes.crashRocketActions}>
      <div className={classes.content}>
        <div className={classes.top}>
          <div className={classes.actionPanel}>
            <button onClick={handleBetDown}>
              <Image src='/icons/minus-btn.svg' width={34} height={34} alt='Минус' />
            </button>

            <p className={classes.value}>{bet}</p>

            <button onClick={handleBetUp}>
              <Image src='/icons/plus-btn.svg' width={34} height={34} alt='Плюс' />
            </button>
          </div>

          <Button borderRadius='medium'>
            Забрать
            <PriceWithCurrency image={{ width: 24, height: 24 }}>
              {new Intl.NumberFormat('de-DE').format(6980)}
            </PriceWithCurrency>
          </Button>

          <div className={classes.actionPanel}>
            <button onClick={handleMultiplierDown}>
              <Image src='/icons/minus-btn.svg' width={34} height={34} alt='Минус' />
            </button>

            <p className={classes.value}>х {multiplier.toFixed(2)}</p>

            <button onClick={handleMultiplierUp}>
              <Image src='/icons/plus-btn.svg' width={34} height={34} alt='Плюс' />
            </button>
          </div>
        </div>

        <div className={classes.bottom}>
          <FormProvider {...useFormProps}>
            <label>
              <Switch className={classes.autoStopSwitch} name='autoStop' /> <span>Автостоп</span>
            </label>
          </FormProvider>
        </div>
      </div>

      <Image
        className={classes.bg}
        src='/img/crash-rocket/crash-rocket-actions-bg.svg'
        width={1131}
        height={189}
        alt='Фон'
      />
    </div>
  )
}
