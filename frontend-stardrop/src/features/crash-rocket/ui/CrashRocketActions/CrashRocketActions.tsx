'use client'

import Image from 'next/image'
import { FormProvider, useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { useState, useEffect } from 'react'

import { Button, PriceWithCurrency, Switch, TextField } from '@/shared/ui'

import classes from './CrashRocketActions.module.scss'

const schema = z.object({
  autoStop: z.boolean(),
  multiplier: z.string(),
  bet: z.string()
})

type FormSchema = z.infer<typeof schema>

export const CrashRocketActions = () => {
  const MAX_BET_VALUE = 5000
  const BET_STEP = 100
  const MULTIPLIER_STEP = 0.5

  const useFormProps = useForm<FormSchema>({
    resolver: zodResolver(schema),
    defaultValues: {
      multiplier: '0',
      bet: '0'
    }
  })
  const { getValues, setValue, watch } = useFormProps
  const betValueWatch = watch('bet')
  const multiplierValueWatch = watch('multiplier')
  const [isBetValueReachMax, setBetValueReachMax] = useState(false)

  useEffect(() => {
    if (Number(betValueWatch) >= MAX_BET_VALUE) {
      setBetValueReachMax(true)
      setValue('bet', `${MAX_BET_VALUE}`)
    } else {
      setBetValueReachMax(false)
    }
  }, [betValueWatch])

  useEffect(() => {
    if (!multiplierValueWatch) {
      setValue('multiplier', '0')
    }
  }, [multiplierValueWatch])

  useEffect(() => {
    if (!betValueWatch) {
      setValue('bet', '0')
    }
  }, [betValueWatch])

  const handleBetDown = (bet: number) => {
    if (bet === 0) return

    if (bet - BET_STEP <= 0) {
      setValue('bet', '0')
      return
    }

    setValue('bet', `${bet - BET_STEP}`)
  }

  const handleBetUp = (bet: number) => {
    if (bet + BET_STEP > MAX_BET_VALUE) {
      setValue('bet', `${MAX_BET_VALUE}`)
      return
    }

    setValue('bet', `${bet + BET_STEP}`)
  }

  const handleMultiplierDown = (multiplier: number) => {
    if (multiplier === 0) return

    if (multiplier - MULTIPLIER_STEP <= 0) {
      setValue('multiplier', '0')
      return
    }

    setValue('multiplier', `${multiplier - MULTIPLIER_STEP}`)
  }

  const handleMultiplierUp = (multiplier: number) => {
    setValue('multiplier', `${multiplier + MULTIPLIER_STEP}`)
  }

  return (
    <div className={classes.crashRocketActions}>
      <div className={classes.content}>
        <FormProvider {...useFormProps}>
          <div className={classes.top}>
            <div className={classes.actionPanel}>
              <button
                onClick={() => {
                  handleBetDown(parseFloat(getValues('bet')))
                }}
              >
                <Image src='/icons/minus-btn.svg' width={34} height={34} alt='Минус' />
              </button>

              <TextField type='number' className={classes.textField} name='bet' />

              <button
                onClick={() => {
                  handleBetUp(parseFloat(getValues('bet')))
                }}
              >
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
              <button
                onClick={() => {
                  handleMultiplierDown(parseFloat(getValues('multiplier')))
                }}
              >
                <Image src='/icons/minus-btn.svg' width={34} height={34} alt='Минус' />
              </button>

              <TextField type='number' className={classes.textField} name='multiplier' />

              <button
                onClick={() => {
                  handleMultiplierUp(parseFloat(getValues('multiplier')))
                }}
              >
                <Image src='/icons/plus-btn.svg' width={34} height={34} alt='Плюс' />
              </button>
            </div>
          </div>

          <div className={classes.bottom}>
            {isBetValueReachMax ? (
              <p className={classes.maxBetValueError}>Максимальная сумма ставки {MAX_BET_VALUE}</p>
            ) : null}

            <label>
              <Switch className={classes.autoStopSwitch} name='autoStop' /> <span>Автостоп</span>
            </label>
          </div>
        </FormProvider>
      </div>

      <Image
        className={classes.bg}
        src='/img/crash-rocket/crash-rocket-actions-bg.svg'
        width={1131}
        height={135}
        alt='Фон'
      />

      <Image
        className={classes.autoStopBg}
        src='/img/crash-rocket/crash-rocket-actions-bg-2.svg'
        width={265}
        height={54}
        alt='Фон'
      />
    </div>
  )
}
