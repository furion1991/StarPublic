'use client'

import Image from 'next/image'
import cn from 'classnames'
import { motion, useAnimation } from 'motion/react'
import { useEffect, useRef } from 'react'

import { LootRarity, LootRarityBox } from '@/entities/loot'

import classes from './CaseRoulette.module.scss'

type Loot = {
  id: string
  name: string
  rarity: LootRarity
  isVisible: boolean
  game: string
  image: string
}

type CaseRouletteProps = {
  droppedItemId: string
  lootItems: Loot[]
  quickOpen: boolean
  onAnimationComplete: () => void
}

export const CaseRoulette = ({
  droppedItemId,
  lootItems,
  quickOpen,
  onAnimationComplete
}: CaseRouletteProps) => {
  const controls = useAnimation()
  const itemRef = useRef<HTMLDivElement>(null)
  const rouletteRef = useRef<HTMLUListElement>(null)
  const QUICK_OPEN_DURATION = 1.5
  const DEFAULT_OPEN_DURATION = 12
  const ANIMATION_DURATION = quickOpen ? QUICK_OPEN_DURATION : DEFAULT_OPEN_DURATION

  const getRouletteItemsList = (lootItems: Loot[]) => {
    const selectedCaseIdx = lootItems.findIndex(({ id }) => droppedItemId === id)

    let tenItemsBeforeDroppedItem = lootItems.slice(selectedCaseIdx - 10, selectedCaseIdx)
    let tenItemsAfterDroppedItem = lootItems.slice(selectedCaseIdx + 1, selectedCaseIdx + 11)

    if (selectedCaseIdx - 10 < 0) {
      const itemsFromStartToSelected = lootItems.slice(0, selectedCaseIdx)
      const itemsFromEnd = lootItems.slice(-10 + itemsFromStartToSelected.length)
      tenItemsBeforeDroppedItem = [...itemsFromEnd, ...itemsFromStartToSelected]
    }

    if (selectedCaseIdx + 10 > lootItems.length) {
      const itemsFromSelectedToEnd = lootItems.slice(
        selectedCaseIdx + 1,
        lootItems.length - selectedCaseIdx
      )
      const itemsFromStart = lootItems.slice(0, 10 - itemsFromSelectedToEnd.length)

      tenItemsAfterDroppedItem = [...itemsFromSelectedToEnd, ...itemsFromStart]
    }

    return [
      ...lootItems.slice(0, lootItems.length / 2),
      ...tenItemsBeforeDroppedItem,
      lootItems[selectedCaseIdx],
      ...tenItemsAfterDroppedItem
    ]
  }

  const getAnimationDistances = (lootItems: Loot[], itemWidth: number, gap: number) => {
    // taking half of the items pool
    // ten blocks before picked item
    // random position of item
    // centered picked item
    // ten blocks after picked item

    if (!itemRef.current) return

    const blockWidth = itemWidth + gap
    const totalItems = lootItems.length

    const centerOfItemDistance = blockWidth / 2
    // skip first 2 items and half of the pointed one
    const initialDistanceSkip = blockWidth * 2 + centerOfItemDistance
    const tenBlockDistance = blockWidth * 10
    const halfOfItemsDistance = (totalItems / 2) * blockWidth - initialDistanceSkip
    const randomPositionOfItem = Math.floor(Math.random() * itemWidth) + 1

    const secondDistance = halfOfItemsDistance + tenBlockDistance + randomPositionOfItem
    const thirdDistance = halfOfItemsDistance + tenBlockDistance + centerOfItemDistance

    return [0, -secondDistance, -thirdDistance]
  }

  useEffect(() => {
    if (!itemRef.current || !rouletteRef.current) return

    const itemWidth = itemRef.current.getBoundingClientRect().width
    const gap = window.getComputedStyle(rouletteRef.current).gap

    controls.start({
      x: getAnimationDistances(lootItems, itemWidth, parseInt(gap)),
      transition: {
        duration: ANIMATION_DURATION,
        visualDuration: ANIMATION_DURATION,
        times: [0, 0.8, 1],
        ease: [
          [0.2, 1, 0.4, 1],
          [0.4, 0.1, 0.4, 1]
        ]
      }
    })
  }, [itemRef, rouletteRef])

  return (
    <div className={classes.caseRoulette}>
      <div className={classes.rouletteContainer}>
        <motion.ul
          ref={rouletteRef}
          className={classes.roulette}
          animate={controls}
          onAnimationStart={() => {
            setTimeout(() => {
              onAnimationComplete()
            }, ANIMATION_DURATION * 1000)
          }}
        >
          {getRouletteItemsList(lootItems).map(({ id, image, name, rarity }, idx) => {
            return (
              <LootRarityBox
                ref={itemRef}
                key={`${id}-${idx}`}
                rarity={rarity}
                className={classes.lootItem}
              >
                <Image src={image} width={140} height={140} alt={name} loading='eager' />

                <p>{name}</p>
              </LootRarityBox>
            )
          })}
        </motion.ul>
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
