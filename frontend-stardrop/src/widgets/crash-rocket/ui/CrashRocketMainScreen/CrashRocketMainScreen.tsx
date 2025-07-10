'use client'

import Image from 'next/image'
import { useEffect, useState } from 'react'
import { motion } from 'motion/react'

import {
  Multiplier,
  MultipliersHistory,
  PrizeWinning,
  CrashRocketGirl
} from '@/entities/crash-rocket'
import { CrashRocketActions } from '@/features/crash-rocket'
import { useCountdown } from '@/shared/hooks'

import { getRandomIntFromInterval } from '@/shared/utils'

import classes from './CrashRocketMainScreen.module.scss'

type GameState = 'next-round-awaiting' | 'round-end' | 'in-play'

const multiplierResult = Number(getRandomIntFromInterval(1, 6).toFixed(2))
const ROUND_TIMEOUT_SECONDS = 10
const ROUND_END_TIMEOUT_MS = 5000

export const CrashRocketMainScreen = () => {
  const [gameState, setGameState] = useState<GameState>('in-play')
  const [multiplier, setMultiplier] = useState<number>(1)
  const { countdown, startCountdown } = useCountdown({
    countdownSeconds: ROUND_TIMEOUT_SECONDS
  })

  useEffect(() => {
    if (multiplier >= multiplierResult && gameState === 'in-play') {
      setGameState('round-end')

      return
    }

    if (gameState === 'round-end') {
      setTimeout(() => {
        setGameState('next-round-awaiting')
        setMultiplier(1)
        startCountdown()
      }, ROUND_END_TIMEOUT_MS)
    }

    if (gameState === 'in-play') {
      startMultiplierGrow()
    }
  }, [gameState, multiplier])

  useEffect(() => {
    if (gameState === 'next-round-awaiting' && countdown === 0) {
      setGameState('in-play')
    }
  }, [gameState, countdown])

  function startMultiplierGrow() {
    setTimeout(() => {
      setMultiplier((multiplier) => Number(Number(multiplier + 0.01)))
    }, 100)
  }

  return (
    <div className={classes.crashRocketMainScreen}>
      <div className={classes.content}>
        <div className={classes.multipliersHistory}>
          <MultipliersHistory />
        </div>

        {gameState === 'in-play' || gameState === 'round-end' ? (
          <>
            <div className={classes.multiplier}>
              <Multiplier multiplier={multiplier} />
            </div>

            <motion.div
              variants={{
                hidden: {
                  opacity: 0,
                  visibility: 'hidden'
                },
                visible: {
                  opacity: 1,
                  visibility: 'visible'
                }
              }}
              transition={{
                duration: 1,
                ease: 'easeInOut'
              }}
              initial='hidden'
              animate={gameState === 'round-end' ? 'visible' : 'hidden'}
              className={classes.prize}
            >
              <PrizeWinning multiplier={2.28} prize={590} />
            </motion.div>

            <CrashRocketGirl gameState={gameState} />

            <Image
              className={classes.bgBall}
              src='/img/crash-rocket/crash-rocket-bg-item.png'
              width={108}
              height={106}
              quality={100}
              alt='Шар'
            />
          </>
        ) : null}

        {gameState === 'next-round-awaiting' ? (
          <div className={classes.nextRoundAwait}>
            <p className={classes.nextRoundLabel}>Ожидание следующего раунда</p>

            <div className={classes.rocket}>
              <Image
                src='/img/crash-rocket/rocket.svg'
                width={102}
                height={314}
                quality={100}
                alt='Ракета'
              />

              <Image
                className={classes.rocketFilled}
                src='/img/crash-rocket/rocket-filled.svg'
                width={102}
                height={314}
                quality={100}
                alt='Ракета'
              />
            </div>

            <p className={classes.countdown}>
              00:00:{countdown < 10 ? `0${countdown}` : countdown}
            </p>
          </div>
        ) : null}

        <Image
          className={classes.mainBg}
          src='/img/crash-rocket/crash-rocket-main-bg.png'
          width={1285}
          height={553}
          quality={100}
          alt='Фон'
        />
      </div>

      <div className={classes.actions}>
        <CrashRocketActions />
      </div>
    </div>
  )
}
