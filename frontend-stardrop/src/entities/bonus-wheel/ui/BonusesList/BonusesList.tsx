'use client'

import Image from 'next/image'

import { useEffect, useState } from 'react'
import { AnimationControls, motion, useAnimation } from 'motion/react'

import { getRandomNumberExcluding, getRandomIntFromInterval } from '@/shared/utils'

import classes from './BonusesList.module.scss'

type BonusWheelAnimationProps = {
  rotation: number
  animationStage: number
  excludedBonuses: number[]
  onRotationComplete: (rotation: number) => void
}

type BonusesListProps = {
  droppedBonus: number
  isAnimationInProcess: boolean
  onAnimationComplete: () => void
}

export const BonusesList = ({
  droppedBonus,
  isAnimationInProcess,
  onAnimationComplete
}: BonusesListProps) => {
  const BONUSES_LIST = Array.from({ length: 7 }).fill(null)
  const BONUS_HIDE_TIME_SEC = 0.5
  const DROPPED_BONUS_ANIMATION_TIME_SEC = 0.5

  const listControls = useAnimation()
  const itemControls = useAnimation()
  const bonusesControls = [
    useAnimation(),
    useAnimation(),
    useAnimation(),
    useAnimation(),
    useAnimation(),
    useAnimation(),
    useAnimation()
  ]

  const [excludedBonuses, setExcludedBonuses] = useState<number[]>([])
  const [animationStage, setAnimationStage] = useState(0)
  const [savedRotationPosition, setSavedRotationPosition] = useState(0)

  useEffect(() => {
    if (!isAnimationInProcess) return

    // here's sort only for temp solution of random stage rotations end
    setExcludedBonuses([...getRandomlyThreeExcludedBonuses(), droppedBonus].sort((a, b) => a - b))
  }, [droppedBonus, isAnimationInProcess])

  useEffect(() => {
    if (!isAnimationInProcess) return

    if (animationStage === 4) {
      setTimeout(() => {
        onAnimationComplete()
      }, DROPPED_BONUS_ANIMATION_TIME_SEC * 1000)

      return
    }

    if (!excludedBonuses.length) return

    startBonusWheelAnimation({
      rotation: savedRotationPosition,
      animationStage,
      excludedBonuses,
      onRotationComplete: (rotation) => {
        if (excludedBonuses[animationStage] === droppedBonus) {
          animateDroppedBonus(bonusesControls[droppedBonus])
          setAnimationStage(4)
          return
        }

        setSavedRotationPosition(rotation)
        setAnimationStage(animationStage + 1)

        if (animationStage === 3) {
          animateDroppedBonus(bonusesControls[droppedBonus])
          return
        }

        hideExcludedBonus(bonusesControls[excludedBonuses[animationStage]])
      }
    })
  }, [isAnimationInProcess, excludedBonuses, animationStage])

  function getRandomlyThreeExcludedBonuses() {
    return Array.from({ length: 3 })
      .map(() => 0)
      .reduce((prev: number[]) => {
        const excludedNumber = getRandomNumberExcluding(prev)
        return [...prev, excludedNumber]
      }, [])
  }

  function startBonusWheelAnimation({
    rotation,
    animationStage,
    excludedBonuses,
    onRotationComplete
  }: BonusWheelAnimationProps) {
    const ROTATIONS_TO_BONUSES = [24, 338, 80, 283, 133, 230, 182]
    const REVERSE_ROTATIONS_TO_BONUSES = [336, 22, 281, 77, 228, 130, 179]
    const TWO_TURNS_DEGREE = 720
    const delay = animationStage === 0 ? 0 : BONUS_HIDE_TIME_SEC
    const isReversedRotation = getRandomIntFromInterval(1, 2) === 1
    const currentBonusIdx = excludedBonuses[animationStage]

    const prevItemRotation =
      animationStage !== 0 ? ROTATIONS_TO_BONUSES[excludedBonuses[animationStage - 1]] : 0
    const rotationToCurrentBonus = ROTATIONS_TO_BONUSES[currentBonusIdx]
    const reverseRotationToCurrentBonus = REVERSE_ROTATIONS_TO_BONUSES[currentBonusIdx]

    const endRotationPosition = isReversedRotation
      ? rotation - TWO_TURNS_DEGREE - reverseRotationToCurrentBonus - prevItemRotation
      : rotation + TWO_TURNS_DEGREE + rotationToCurrentBonus - prevItemRotation

    listControls
      .start({
        rotate: endRotationPosition,
        transition: {
          duration: 2,
          ease: 'easeInOut',
          delay
        }
      })
      .then(() => {
        onRotationComplete(endRotationPosition)
      })

    // item content reversed rotation for container rotation compensate
    itemControls.start({
      rotate: -endRotationPosition,
      transition: {
        duration: 2,
        ease: 'easeInOut',
        delay
      }
    })
  }

  function hideExcludedBonus(controls: AnimationControls) {
    controls.start({
      transform: 'scale(0.85)',
      opacity: 0,
      transition: {
        duration: BONUS_HIDE_TIME_SEC,
        ease: 'easeOut'
      }
    })
  }

  function animateDroppedBonus(controls: AnimationControls) {
    controls.start({
      transform: ['scale(1)', 'scale(1.3)', 'scale(1.1)'],
      transition: {
        ease: 'easeOut',
        duration: DROPPED_BONUS_ANIMATION_TIME_SEC,
        times: [0, 0.5, 1]
      }
    })
  }

  return (
    <motion.ul animate={listControls} className={classes.bonusesList}>
      {BONUSES_LIST.map((_, idx) => {
        return (
          <motion.li key={idx} animate={itemControls} className={classes.bonusItem}>
            <motion.div
              initial={{
                opacity: 1,
                transform: 'scale(1)'
              }}
              animate={bonusesControls[idx]}
            >
              <Image
                src='/placeholders/bonus-wheel-loot.png'
                width={224}
                height={236}
                alt='Бонус'
              />
            </motion.div>
          </motion.li>
        )
      })}
    </motion.ul>
  )
}
