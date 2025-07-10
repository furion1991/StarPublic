'use client'

import { useEffect, useState } from 'react'
import cn from 'classnames'

import classes from './Timer.module.scss'

type Timer = {
  days: string | number
  hours: string | number
  minutes: string | number
  seconds: string | number
}

type StyleVariant = 1 | 2

type TimerProps = {
  expiryTime: string
  styleVariant?: StyleVariant
}

export const Timer = ({ expiryTime, styleVariant = 1 }: TimerProps) => {
  const [countdownTime, setCountdownTime] = useState<Timer>({
    days: 0,
    hours: 0,
    minutes: 0,
    seconds: 0
  })

  function getRemainingTime(expiryTime: string) {
    const countdownDateTime = new Date(expiryTime).getTime()
    const currentTime = new Date().getTime()
    const remainingDayTime = countdownDateTime - currentTime
    const totalDays = Math.floor(remainingDayTime / (1000 * 60 * 60 * 24))
    const totalHours = Math.floor((remainingDayTime % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60))
    const totalMinutes = Math.floor((remainingDayTime % (1000 * 60 * 60)) / (1000 * 60))
    const totalSeconds = Math.floor((remainingDayTime % (1000 * 60)) / 1000)

    return {
      totalDays,
      totalHours,
      totalMinutes,
      totalSeconds,
      remainingDayTime
    }
  }

  useEffect(() => {
    const updateTime = () => {
      const { totalDays, totalHours, totalMinutes, totalSeconds } = getRemainingTime(expiryTime)

      setCountdownTime({
        days: totalDays,
        hours: totalHours,
        minutes: totalMinutes,
        seconds: totalSeconds
      })
    }

    updateTime()

    const interval = setInterval(() => {
      updateTime()
    }, 1000)

    return () => clearInterval(interval)
  }, [expiryTime])

  return (
    <div
      className={cn(classes.timer, {
        [classes.secondary]: styleVariant === 2
      })}
    >
      <div className={classes.field}>
        <p>{countdownTime.days}</p>
        <p>Дней</p>
      </div>

      <div className={classes.field}>
        <p>{countdownTime.hours}</p>
        <p>Часов</p>
      </div>

      <div className={classes.field}>
        <p>{countdownTime.minutes}</p>
        <p>Минут</p>
      </div>

      <div className={classes.field}>
        <p>{countdownTime.seconds}</p>
        <p>Секунд</p>
      </div>
    </div>
  )
}
