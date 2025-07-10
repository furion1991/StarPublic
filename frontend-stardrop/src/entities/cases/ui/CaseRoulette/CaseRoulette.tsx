'use client'

import Image from 'next/image'
import cn from 'classnames'
import { motion, useAnimation } from 'motion/react'
import { useState, useEffect, useRef } from 'react'

import { type Loot, LootRarityBox } from '@/entities/loot'

import { getRandomIntFromInterval } from '@/shared/utils'

import classes from './CaseRoulette.module.scss'

type DroppedLoot = Loot & {
  inventoryId: string
}

type CaseRouletteProps = {
  droppedItem: DroppedLoot
  lootItems: Loot[]
  quickAnimation: boolean
  onAnimationComplete: () => void
}

export const CaseRoulette = ({
  droppedItem,
  lootItems,
  quickAnimation,
  onAnimationComplete
}: CaseRouletteProps) => {
  const QUICK_ANIMATION_DURATION = 1.5
  const DEFAULT_ANIMATION_DURATION = 10
  const ANIMATION_DURATION = quickAnimation ? QUICK_ANIMATION_DURATION : DEFAULT_ANIMATION_DURATION

  const controls = useAnimation()
  const itemRef = useRef<HTMLDivElement>(null)
  const rouletteRef = useRef<HTMLUListElement>(null)

  const [isAnimationCompleted, setAnimationCompleted] = useState(false)

  useEffect(() => {
    if (!itemRef.current) return
    if (!rouletteRef.current) return

    const itemWidth = itemRef.current.getBoundingClientRect().width
    const gap = window.getComputedStyle(rouletteRef.current).gap
    const lootItemsNumberWithoutDropped = lootItems.filter(({ id }) => id !== droppedItem.id).length

    const animationDistances = getAnimationDistances(
      lootItemsNumberWithoutDropped,
      itemWidth,
      parseInt(gap)
    )
    const lastDistance = animationDistances[animationDistances.length - 1]

    controls
      .start({
        x: animationDistances,
        transition: {
          duration: ANIMATION_DURATION,
          times: [0, 0.9, 1],
          ease: [
            [0.2, 0.8, 0.4, 1],
            [0.4, 0.1, 0.4, 1]
          ]
        }
      })
      .then(() => {
        onAnimationComplete()
        setAnimationCompleted(true)

        const correctionDistance = window.innerWidth >= 600 ? 35 : 20

        controls.start({
          x: [lastDistance, lastDistance - correctionDistance],

          transition: {
            duration: 1,
            ease: 'easeOut'
          }
        })
      })
  }, [])

  function getRouletteList(droppedItem: Loot, lootItems: Loot[]) {
    const droppedItemIdx = lootItems.findIndex(({ id }) => droppedItem.id === id)
    const itemsWithoutDropped = lootItems.filter(({ id }) => id !== droppedItem.id)

    const halfItems = itemsWithoutDropped.slice(0, Math.floor(itemsWithoutDropped.length / 2))

    const tenItemsBeforeDroppedItem = [...lootItems, ...lootItems, ...lootItems].slice(
      lootItems.length + droppedItemIdx - 10,
      lootItems.length + droppedItemIdx
    )

    const tenItemsAfterDroppedItem = [...lootItems, ...lootItems, ...lootItems].slice(
      lootItems.length + droppedItemIdx + 1,
      lootItems.length + droppedItemIdx + 1 + 10
    )

    return [...halfItems, ...tenItemsBeforeDroppedItem, droppedItem, ...tenItemsAfterDroppedItem]
  }

  function getAnimationDistances(itemsNumber: number, itemWidth: number, gap: number) {
    // taking half of the items pool
    // ten blocks before picked item
    // random position of item
    // centered picked item
    // ten blocks after picked item
    const blockWidth = itemWidth + gap
    const halfOfItemsLength = Math.floor(itemsNumber / 2)

    const centerOfItemDistance = blockWidth / 2
    // skip first 2 items and half of the pointed one
    const initialDistanceSkip = blockWidth * 2 + centerOfItemDistance
    const tenBlockDistance = blockWidth * 10
    const halfOfItemsDistance = halfOfItemsLength * blockWidth - initialDistanceSkip
    const randomPositionOfItem = getRandomIntFromInterval(25, itemWidth - 25)

    const secondDistance = halfOfItemsDistance + tenBlockDistance + randomPositionOfItem
    const thirdDistance = halfOfItemsDistance + tenBlockDistance + centerOfItemDistance

    return [0, -secondDistance, -thirdDistance]
  }

  return (
    <div className={classes.caseRoulette}>
      <div className={classes.rouletteContainer}>
        <motion.ul className={classes.roulette} ref={rouletteRef} animate={controls}>
          {getRouletteList(droppedItem, lootItems).map(({ id, image, name, rarity }, idx) => {
            const isDroppedItem = droppedItem.id === id

            return (
              <li
                key={`${id}-${idx}`}
                className={cn(classes.lootItemContainer, {
                  [classes.dropped]: isDroppedItem && isAnimationCompleted
                })}
              >
                <LootRarityBox
                  key={`${id}-${idx}`}
                  ref={itemRef}
                  rarity={rarity}
                  className={cn(classes.lootItem, {
                    [classes.dropped]: isDroppedItem && isAnimationCompleted
                  })}
                >
                  <Image src={image} width={140} height={140} alt={name} />

                  <p>{name}</p>
                </LootRarityBox>
              </li>
            )
          })}
        </motion.ul>
      </div>

      <div
        className={cn(classes.blackout, {
          [classes.wider]: isAnimationCompleted
        })}
      >
        <div />
        <div />
      </div>

      <div className={classes.triangles}>
        <div className={cn(classes.triangle, classes.triangleTop)}>
          <Image src='/icons/triangle-right.svg' width={28} height={28} alt='Треугольник' />
        </div>

        <div className={cn(classes.triangle, classes.triangleBottom)}>
          <Image src='/icons/triangle-right.svg' width={28} height={28} alt='Треугольник' />
        </div>
      </div>
    </div>
  )
}
