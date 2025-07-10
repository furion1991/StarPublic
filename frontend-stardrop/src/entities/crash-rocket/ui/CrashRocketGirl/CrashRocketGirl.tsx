'use client'

import Image from 'next/image'
import { useEffect, useRef, useState } from 'react'
import { motion, useMotionValue } from 'motion/react'

import { getRandomIntFromInterval } from '@/shared/utils'

import classes from './CrashRocketGirl.module.scss'

type GameState = 'next-round-awaiting' | 'round-end' | 'in-play'

type CrashRocketGirlProps = {
  gameState: GameState
}

export const CrashRocketGirl = ({ gameState }: CrashRocketGirlProps) => {
  const screenWidth = useState(window.innerWidth)
  const isMobile = screenWidth[0] <= 500

  const START_POSITION_X = !isMobile ? -500 : -150
  const START_POSITION_Y = 250
  const FLYING_POSITION_X = 144
  const FLYING_POSITION_Y = 150

  const girlBgX = useMotionValue(START_POSITION_X)
  const girlBgY = useMotionValue(START_POSITION_Y)
  const animationIntervalRef = useRef<NodeJS.Timeout | null>(null)

  const animateMovement = () => {
    girlBgX.set(getRandomIntFromInterval(FLYING_POSITION_X - 25, FLYING_POSITION_X + 25))
    girlBgY.set(getRandomIntFromInterval(FLYING_POSITION_Y - 25, FLYING_POSITION_Y + 25))
  }

  const flewAwayAnimate = () => {
    girlBgX.set(girlBgX.get() + 1500)
    girlBgY.set(girlBgY.get() - 300)
  }

  useEffect(() => {
    if (gameState === 'in-play') {
      if (animationIntervalRef.current) {
        clearInterval(animationIntervalRef.current)
      }

      if (girlBgX.get() === START_POSITION_X && girlBgY.get() === START_POSITION_Y) {
        animateMovement()
      }

      animationIntervalRef.current = setInterval(() => {
        animateMovement()
      }, 500)

      return
    }

    if (gameState === 'round-end' && animationIntervalRef.current) {
      clearInterval(animationIntervalRef.current)
      flewAwayAnimate()
      return
    }

    return () => {
      if (animationIntervalRef.current) {
        clearInterval(animationIntervalRef.current)
      }
    }
  }, [gameState])

  return (
    <motion.div
      className={classes.bgGirl}
      style={{
        x: girlBgX,
        y: girlBgY
      }}
    >
      <Image
        src='/img/crash-rocket/crash-rocket-girl.png'
        width={756}
        height={300}
        quality={100}
        alt='Девушка'
      />
    </motion.div>
  )
}
