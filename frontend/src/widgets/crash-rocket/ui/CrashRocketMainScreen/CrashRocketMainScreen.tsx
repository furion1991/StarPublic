'use client'

import Image from 'next/image'
import { useEffect, useState } from 'react'

import { Multiplier, MultipliersHistory, PrizeWinning } from '@/entities/crash-rocket'
import { CrashRocketActions } from '@/features/crash-rocket'

import classes from './CrashRocketMainScreen.module.scss'

type GameState = 'next-round-awaiting' | 'in-play'
type UserStatus = 'win' | 'loose'

export const CrashRocketMainScreen = () => {
  const [gameState, setGameState] = useState<GameState>('in-play')
  const [userStatus, setUserStatus] = useState<UserStatus>()
  const [multiplier, setMultiplier] = useState<number>(0)
  const [rarity, setRarity] = useState<number>(0)

  useEffect(() => {
    setMultiplier(Number(randomIntFromInterval(1, 10).toFixed(2)))
    setRarity(Math.floor(randomIntFromInterval(0, 3)))
  }, [])

  function randomIntFromInterval(min: number, max: number) {
    return Math.random() * (max - min + 1) + min
  }

  return (
    <div className={classes.crashRocketMainScreen}>
      <div className={classes.content}>
        {gameState === 'in-play' ? (
          <>
            <MultipliersHistory />

            <div className={classes.multiplier}>
              <Multiplier multiplier={multiplier} rarity={rarity} />
            </div>

            <div className={classes.prize}>
              <PrizeWinning multiplier={2.28} prize={590} />
            </div>
          </>
        ) : null}

        {gameState === 'next-round-awaiting' ? (
          <div className={classes.nextRoundAwait}>
            <p className={classes.nextRoundLabel}>Ожидание следующего раунда</p>

            <Image
              src='/img/crash-rocket/rocket.png'
              width={102}
              height={311}
              quality={100}
              alt='Ракета'
            />

            <p className={classes.countdown}>00:00:15</p>
          </div>
        ) : null}
      </div>

      <div className={classes.actions}>
        <CrashRocketActions />
      </div>

      <Image
        className={classes.mainBg}
        src='/img/crash-rocket/crash-rocket-main-bg.png'
        width={1285}
        height={553}
        quality={100}
        alt='Фон'
      />

      {gameState === 'in-play' ? (
        <>
          {' '}
          <Image
            className={classes.bgGirl}
            src='/img/crash-rocket/crash-rocket-girl.png'
            width={755}
            height={300}
            quality={100}
            alt='Девушка'
          />
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
    </div>
  )
}
