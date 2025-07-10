'use client'

import Image from 'next/image'
import cn from 'classnames'
import { motion, useAnimation } from 'motion/react'
import { useState, useEffect, useRef } from 'react'

import { type Loot, LootRarityBox } from '@/entities/loot'
import { Button, PriceWithCurrency } from '@/shared/ui'

import classes from './CaseRouletteVertical.module.scss'

type DroppedLoot = Loot & {
  inventoryId: string
}

type CaseRouletteVerticalProps = {
  isAllItemsSold: boolean
  droppedItems: DroppedLoot[]
  lootItems: Loot[]
  quickAnimation: boolean
  onAnimationComplete: () => void
  onItemSell: (inventoryId: string) => void
}

export const CaseRouletteVertical = ({
  isAllItemsSold,
  droppedItems,
  lootItems,
  quickAnimation,
  onAnimationComplete,
  onItemSell
}: CaseRouletteVerticalProps) => {
  const QUICK_ANIMATION_DURATION = 1.5
  const DEFAULT_ANIMATION_DURATION = 4
  const ANIMATION_DURATION = quickAnimation ? QUICK_ANIMATION_DURATION : DEFAULT_ANIMATION_DURATION

  const rouletteRef = useRef<HTMLUListElement>(null)
  const itemRef = useRef<HTMLDivElement>(null)
  const controls = useAnimation()

  const [soldItems, setSoldItems] = useState<string[]>([])

  useEffect(() => {
    if (!itemRef.current || !rouletteRef.current) return

    const itemHeight = itemRef.current.getBoundingClientRect().height
    const gap = window.getComputedStyle(rouletteRef.current).gap

    controls
      .start({
        y: [0, -getAnimationDistance(lootItems, itemHeight, parseInt(gap))]
      })
      .then(() => {
        onAnimationComplete()
      })
  }, [])

  function getAnimationDistance(lootItems: Loot[], itemHeight: number, gap: number) {
    const blockHeight = itemHeight + gap
    const halfOfItems = Math.floor(lootItems.length / 2)

    return blockHeight * halfOfItems
  }

  return (
    <div className={classes.caseRouletteVertical}>
      <div
        className={classes.container}
        style={{
          gridTemplateColumns: `repeat(${droppedItems.length}, 1fr)`
        }}
      >
        {Array.from({ length: droppedItems.length }).map((_, listIdx) => {
          const delay = !quickAnimation ? ANIMATION_DURATION + listIdx : 0

          return (
            <motion.ul
              key={listIdx}
              ref={rouletteRef}
              className={classes.roulette}
              animate={controls}
              transition={{
                duration: !quickAnimation ? ANIMATION_DURATION + listIdx : ANIMATION_DURATION,
                ease: [0.2, 0.8, 0.4, 1]
              }}
            >
              {lootItems
                .slice(0, lootItems.length / 2)
                .concat(droppedItems[listIdx])
                .map((item, idx, arr) => {
                  const { id, image, name, rarity, sellPrice } = item
                  const isDroppedItem = arr.length - 1 === idx
                  const isItemSold = soldItems.includes(droppedItems[listIdx].inventoryId)

                  return (
                    <LootRarityBox
                      ref={itemRef}
                      key={`${id}-${idx}`}
                      rarity={rarity}
                      className={cn(classes.lootItem, {
                        [classes.dropped]: isDroppedItem
                      })}
                    >
                      {isDroppedItem ? (
                        <motion.div
                          className={classes.sellPriceContainer}
                          initial={{ y: -20, opacity: 0, visibility: 'hidden' }}
                          animate={{ y: 0, opacity: 1, visibility: 'visible' }}
                          transition={{
                            duration: 1,
                            delay
                          }}
                        >
                          <PriceWithCurrency>
                            {new Intl.NumberFormat('de-DE').format(sellPrice)}
                          </PriceWithCurrency>
                        </motion.div>
                      ) : null}

                      <Image src={image} width={140} height={140} alt={name} />

                      <p>{name}</p>

                      {isDroppedItem ? (
                        <motion.div
                          className={classes.sellBtnContainer}
                          initial={{ y: 20, opacity: 0, visibility: 'hidden' }}
                          animate={{ y: 0, opacity: 1, visibility: 'visible' }}
                          transition={{
                            duration: 1,
                            delay
                          }}
                        >
                          <Button
                            className={classes.sellBtn}
                            disabled={isItemSold || isAllItemsSold}
                            onClick={() => {
                              const { inventoryId } = droppedItems[listIdx]
                              onItemSell(inventoryId)
                              setSoldItems([...soldItems, inventoryId])
                            }}
                          >
                            {!isItemSold && !isAllItemsSold ? 'Продать' : 'Продано'}{' '}
                            <Image
                              src='/icons/wallet-white.svg'
                              width={24}
                              height={24}
                              alt='Кошелек'
                            />
                          </Button>
                        </motion.div>
                      ) : null}
                    </LootRarityBox>
                  )
                })}
            </motion.ul>
          )
        })}
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
